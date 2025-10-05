using Microsoft.Xna.Framework;
using System;

namespace conscious
{
    /// <summary>Class <c>ChangeRoomCommand</c> holds data and logic
    /// to change the Room with a blending effect.
    /// </summary>
    ///
    public class ChangeRoomCommand : Command
    {
        Vector2 _initPlayerPosition;
        PlayerState _initPlayerState;
        int _nextRoomId;
        RoomManager _roomManager;

        public ChangeRoomCommand(Vector2 startPosition, PlayerState animState,
                                 int nextRoomId, RoomManager roomManager,
                                 int thingId = 0) : base(thingId)
        {
            _initPlayerPosition = startPosition;
            _initPlayerState = animState;
            _nextRoomId = nextRoomId;
            _roomManager = roomManager;
        }

        public override void ExecuteCommand(GameTime gameTime, Thing thing)
        {

            // if (_initPlayerPosition != Vector2.Zero)
            //     thing.Position = _initPlayerPosition;

            if (GlobalData.IsSameOrSubclass(typeof(Player), thing.GetType()))
            {
                Player player = (Player)thing;

                _roomManager.changeRoom(_nextRoomId, _initPlayerPosition);
                player.PlayerState = _initPlayerState;
                CommandFinished = true;
            }
            else
            {
                throw new ArgumentException("Thing is not a Player or Entity");
            }
        }

        public override DataHolderCommand GetDataHolderCommand()
        {
            DataHolderChangeRoomCommand dataHolderCommand = new DataHolderChangeRoomCommand();
            dataHolderCommand = (DataHolderChangeRoomCommand)base.GetDataHolderCommand(dataHolderCommand);
            return FillDataHolder(dataHolderCommand);
        }

        public DataHolderCommand GetDataHolderCommand(DataHolderChangeRoomCommand dataHolderCommand)
        {
            dataHolderCommand = (DataHolderChangeRoomCommand)base.GetDataHolderCommand(dataHolderCommand);
            return FillDataHolder(dataHolderCommand);
        }
        
        private DataHolderCommand FillDataHolder(DataHolderChangeRoomCommand dataHolderCommand)
        {
            dataHolderCommand.StartPositionX = _initPlayerPosition.X;
            dataHolderCommand.StartPositionY = _initPlayerPosition.Y;
            dataHolderCommand.AnimState = _initPlayerState.ToString();
            dataHolderCommand.NextRoomId = _nextRoomId;
            return dataHolderCommand;
        }
    }
}