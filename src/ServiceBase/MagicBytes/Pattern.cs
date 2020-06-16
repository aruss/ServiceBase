namespace ServiceBase.MagicBytes
{
    public class Pattern
    {
        #region Constructors

        public Pattern(
            byte[] signature,
            long offset,
            string mimeType,
            string fileExtension,
            string name)
        {
            this.Signature = signature;
            this.Offset = offset;
            this.MimeType = mimeType;
            this.FileExtension = fileExtension;
            this.Name = name;
        }

        #endregion Constructors

        #region Public Properties

        public byte[] Signature { get; set; }

        public long Offset { get; set; }

        public string MimeType { get; set; }

        public string FileExtension { get; set; }

        public string Name { get; set; }

        #endregion Public Properties
    }
}