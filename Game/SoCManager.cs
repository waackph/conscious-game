using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace conscious
{
    /// <summary>Class <c>SoCManager</c> implements a thought dialog system.
    /// It handles the stream of consciousness that is represented in a thought tree that is  
    /// triggered by a clicked Item for the protagonist.
    /// </summary>
    ///
    public class SoCManager : IComponent
    {
        private MoodStateManager _moodStateManager;
        private AudioManager _audioManager;
        public Queue<ThoughtNode> Thoughts { get; private set; }
        Random random = new Random();
        public int _maxThoughts;
        private List<ThoughtLink> _currentSubthoughtLinks;
        private List<ThoughtNode> _randomThoughts;
        private ThoughtNode _currentThought;
        private FinalThoughtLink _finalOption;
        private List<int> _toUnlock = new List<int>();
        private List<int> _alreadyUnlocked = new List<int>();
        private bool _isStart;
        private int _timeSinceLastRandomThought;
        private int _timeToWait;
        private int _maxMinutesToWait;
        
        public bool IsInThoughtMode;
        public event EventHandler<VerbActionEventArgs> ActionEvent;
        public event EventHandler<ThoughtNode> AddThoughtEvent;
        public event EventHandler<bool> FinishInteractionEvent;
        public event EventHandler<FinalEdgeEventArgs> FinalEdgeSelected;
        public Verb VerbResult { get; private set; }

        public SoCManager(MoodStateManager moodStateManager, AudioManager audioManager)
        {
            _moodStateManager = moodStateManager;
            _audioManager = audioManager;
            Thoughts = new Queue<ThoughtNode>();
            _maxThoughts = 3;
            VerbResult = Verb.None;

            _randomThoughts = new List<ThoughtNode>();
            _randomThoughts.Add(new ThoughtNode(1000, "War? What is it good for?", 0, true, 0));
            _randomThoughts.Add(new ThoughtNode(1001, "I never died in my sleep. Such a shame.", 0, true, 0));
            _randomThoughts.Add(new ThoughtNode(1002, "Dadada dada dadada", 0, true, 0));
            _randomThoughts.Add(new ThoughtNode(1003, "I don't like the ambience right now.", 0, true, 0));
            _randomThoughts.Add(new ThoughtNode(1004, "The Avatar movie was endlessly overhyped.", 0, true, 0));

            _isStart = true;
            _maxMinutesToWait = 30;

            _timeToWait = 0;
            _timeSinceLastRandomThought = 0;

            IsInThoughtMode = false;
        }

        public void Update(GameTime gameTime)
        {
            _timeSinceLastRandomThought += gameTime.ElapsedGameTime.Milliseconds;
            // logic to add thoughts randomly some times
            if(_isStart)
            {
                drawRandomThought();
                _isStart = false;
                _timeToWait = drawTimeInterval();
            }
            else if (_timeSinceLastRandomThought > _timeToWait & !IsInThoughtMode)
            {
                _timeSinceLastRandomThought = 0;
                _timeToWait = drawTimeInterval();
                drawRandomThought();
            }
        }

        public void drawRandomThought()
        {
            int randomIndex = random.Next(0, _randomThoughts.Count);
            AddThought(_randomThoughts[randomIndex]);
        }

        public int drawTimeInterval()
        {
            int intervalInMinutes = random.Next(5, _maxMinutesToWait) * 1000 * 60;
            return intervalInMinutes;
        }

        public void Draw(SpriteBatch spriteBatch){ }

        public void AddThought(ThoughtNode thought)
        {
            // check if there are links to unlock in the newly selected thought tree 
            checkUnlockIds(thought);
            if(!containsThoughtNode(Thoughts, thought))
            {
                if(Thoughts.Count + 1 > _maxThoughts)
                {
                    Thoughts.Dequeue();
                }
                Thoughts.Enqueue(thought);
                // If thought has sound, play it
                if(thought.EventSound != null)
                {
                    _audioManager.PlaySoundEffect(thought.EventSound, true);
                }

                // Invoke event for UiDisplayThoughtManager to add the thought UI Element as well
                OnAddThoughtEvent(thought);
            }
        }

        private bool containsThoughtNode(Queue<ThoughtNode> thoughts, ThoughtNode node)
        {
            foreach(ThoughtNode thought in thoughts)
            {
                if(thought.Thought == node.Thought)
                {
                    return true;
                }
            }
            return false;
        }

        public ThoughtNode SelectThought(string thoughtName)
        {
            ThoughtNode node = GetThought(thoughtName);
            // If Sound is active and it is soundname of node, then stop sound
            _audioManager.StopSoundEffect(node.EventSound);

            checkUnlockIds(node);
            // If thought is an Selectable Thought: choose link from root
            if(node.HasLinks())
            {
                if(node.IsRoot)
                {
                    _currentThought = node;
                    node.Links.Sort((x, y) => x.Id.CompareTo(y.Id));
                    foreach(ThoughtLink link in node.Links)
                    {
                        if(!link.IsLocked && link.MoodValid(_moodStateManager.moodState))
                        {
                            ThoughtNode displayNode = link.NextNode;
                            _currentSubthoughtLinks = displayNode.Links;
                            // _uiDisplayThought.StartThoughtMode(displayNode, displayNode.Links);
                            return displayNode;
                        }
                    }
                }
            }
            return null;
        }

        public bool IsSuccessEdgeChosen(string thoughtName)
        {
            ThoughtLink option = GetOption(thoughtName);
            if(option != null && !option.IsLocked && option.MoodValid(_moodStateManager.moodState))
            {
                if(typeof(FinalThoughtLink) == option.GetType())
                {
                    FinalThoughtLink finalOption = (FinalThoughtLink)option;
                    return finalOption.IsSuccessEdge;
                }
            }
            return false;
        }

        public ThoughtNode SelectSubthought(string thoughtName)
        {
            ThoughtLink option = GetOption(thoughtName);
            if(option != null && !option.IsLocked && option.MoodValid(_moodStateManager.moodState))
            {
                ThoughtNode node = option.NextNode;
                if(node == null || !node.HasLinks())
                {
                    // If there is a last node (without links), it should be displayed as a concluding thought in the SoC Main Window
                    if(node != null && !node.HasLinks())
                    {
                        AddThought(node);
                    }
                    // If the link is a final option, execute possible operations
                    if(typeof(FinalThoughtLink) == option.GetType())
                    {
                        _finalOption = (FinalThoughtLink)option;
                        FinalEdgeEventArgs finalEdgeData = new FinalEdgeEventArgs();
                        finalEdgeData.verbAction = _finalOption.Verb;
                        finalEdgeData.seq = _finalOption.ThoughtSequence;
                        finalEdgeData.EdgeMood = _finalOption.MoodChange;
                        finalEdgeData.EventThoughtId = _finalOption.EventThoughtId;
                        OnFinalEdgeSelected(finalEdgeData);
                        if(_finalOption.Verb != Verb.None && _finalOption.Verb != Verb.WakeUp)
                        {
                            VerbActionEventArgs data = new VerbActionEventArgs();
                            data.ThingId = _currentThought.ThingId;
                            data.verbAction = _finalOption.Verb;
                            OnActionEvent(data);
                        }
                        else
                        {
                            bool usedThought;
                            if(_finalOption.IsSuccessEdge && !_finalOption.IsLocked)
                            {
                                usedThought = true;
                                _currentThought.IsUsed = true;
                            }
                            else
                            {
                                usedThought = false;
                            }
                            if(_finalOption.UnlockId != 0)
                                unlockThoughtLink(_finalOption.UnlockId);
                            OnFinishInteractionEvent(usedThought);
                            option.IsVisited = true;
                            if(_finalOption.ThoughtSequence == null)
                                _moodStateManager.StateChange = _finalOption.MoodChange;
                        }
                    }
                    // _uiDisplayThought.EndThoughtMode();
                    return null;
                }
                else
                {
                    _currentSubthoughtLinks = node.Links;
                    option.IsVisited = true;
                    // _uiDisplayThought.ChangeSubthought(node, node.Links);
                    return node;
                }
            }
            return null;
        }

        public void InteractionIsSuccessfull(bool isCanceled)
        {
            bool successEdge = (!isCanceled && _finalOption.IsSuccessEdge);
            OnFinishInteractionEvent(successEdge);
            if(!isCanceled)
            {
                _finalOption.IsVisited = true;
                _currentThought.IsUsed = true;
                if(_finalOption.UnlockId != 0)
                    unlockThoughtLink(_finalOption.UnlockId);
                if(_finalOption.MoodChange != MoodState.None)
                    _moodStateManager.StateChange = _finalOption.MoodChange;
            }
                _finalOption = null;
                _currentThought = null;
        }

        private void checkUnlockId(ThoughtNode node, int unlockId)
        {
            if(node != null)
            {
                foreach(ThoughtLink link in node.Links)
                {
                    if(link.Id == unlockId)
                    {
                        link.IsLocked = false;
                        _toUnlock.Remove(unlockId);
                        _alreadyUnlocked.Add(unlockId);
                        break;
                    }
                    else
                    {
                        checkUnlockId(link.NextNode, unlockId);
                    }
                }
            }
        }

        private void unlockThoughtLink(int unlockId)
        {
            if(!_alreadyUnlocked.Contains(unlockId))
            {
                // store id to unlock to be able to later unlock a link from a different tree
                _toUnlock.Add(unlockId);
                // first traverse own tree
                checkUnlockId(_currentThought, unlockId);
            }
        }

        private void checkUnlockIds(ThoughtNode node)
        {
            List<int> tempUnlockIds = new List<int>(_toUnlock);
            if(tempUnlockIds.Count != 0)
            {
                foreach(int id in tempUnlockIds)
                {
                    checkUnlockId(node, id);
                }
            }
        }

        public void ThoughtModeStart()
        {
            IsInThoughtMode = true;
        }

        public void ThoughtModeFinish()
        {
            IsInThoughtMode = false;
        }

        protected virtual void OnActionEvent(VerbActionEventArgs e)
        {
            ActionEvent?.Invoke(this, e);
        }

        protected virtual void OnAddThoughtEvent(ThoughtNode e)
        {
            AddThoughtEvent?.Invoke(this, e);
        }

        protected virtual void OnFinishInteractionEvent(bool e)
        {
            FinishInteractionEvent?.Invoke(this, e);
        }

        protected virtual void OnFinalEdgeSelected(FinalEdgeEventArgs e)
        {
            FinalEdgeSelected?.Invoke(this, e);
        }

        private Node FindLinkById(int id)
        {
            throw new NotImplementedException();
        }

        public ThoughtNode GetThought(string thoughtText)
        {
            foreach(ThoughtNode thought in Thoughts)
            {
                if(thought.Thought == thoughtText)
                {
                    return thought;
                }
            }
            return null;
        }

        public ThoughtLink GetOption(string thoughtText)
        {
            if(_currentSubthoughtLinks != null)
            {
                foreach(ThoughtLink thought in _currentSubthoughtLinks)
                {
                    if(thought.Option == thoughtText)
                    {
                        return thought;
                    }
                }
            }
            return null;
        }
    }
}
