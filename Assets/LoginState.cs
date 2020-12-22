namespace DefaultNamespace
{
    public static class LoginState
    {
        public static string VideoUrl = "noVideo";
        public static VideoState VideoState = VideoState.NotSet;
    }

    public enum VideoState
    {
        NotSet = 0,
        VideoOk = 1,
        HasToPay = 2,
        ShowFinished = 3
    }
}