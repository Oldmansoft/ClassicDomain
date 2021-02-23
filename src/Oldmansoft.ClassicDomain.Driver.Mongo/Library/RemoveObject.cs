namespace Oldmansoft.ClassicDomain.Driver.Mongo.Library
{
    class RemoveObject
    {
        public string _specail { get; set; }

        private RemoveObject()
        {
            _specail = "remove";
        }

        public static readonly RemoveObject Instance = new RemoveObject();
    }
}
