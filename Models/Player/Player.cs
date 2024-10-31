
namespace Chess.Models.Player
{
    class Player
    {
        #region Fields

        private string _username = default!;
        public string Username { get; set; }

        private string _password = default!;
        public string Password { get; set; }

        private int _elo = default!;
        public int Elo { get; set; }

        private bool _isWhite = default!;
        public bool IsWhite { get; set; }


        #endregion

        public Player()
        {

        }
    }
}
