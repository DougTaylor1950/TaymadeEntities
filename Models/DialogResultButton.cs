namespace TaymadeEntities.Models
{
    public class DialogResultButton : ModelBase
    {
        private ResultType result = ResultType.Ok;

        public enum ResultType
        {
            Ok,
            Cancel
        }

        public ResultType Result { get => result; set => result = value; }

        public PhraseEntry? PhraseEntry { get; set; }

        public PhraseEntry? SubPhraseEntry { get; set; }

        public string? Paramater { get; set; }

        public int? Code { get; set; }

        public string? Command { get; set; }

        public int? Seconds { get; set; }
    }
}
