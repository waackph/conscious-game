namespace conscious
{
    public static class GlobalData
    {
        public static int ScreenWidth = 1920;
        public static int ScreenHeight = 1080;
        public static int InitRoomId = 3;
        public static int PlayerDrawOrder = 10;

        public static bool IsNotBackgroundOrPlayer(Entity entity)
        {
            // If thing is not a background (min 1920 in width)
            return entity.Width < 1900 && !entity.Name.ToLower().Contains("background") && !entity.Name.ToLower().Contains("hintergrund") && entity.Name != "Player";
        }
    }

}
