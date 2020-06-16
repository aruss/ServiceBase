namespace ServiceBase.MagicBytes
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents a assistent used for identifying files by parsing the magic
    /// bytes.
    /// </summary>
    public static class Wizard
    {
        #region Private Static Fields

        /// <summary>
        /// Saves all patterns used for identifying file types.
        /// </summary>
        private static List<Pattern> _patterns;

        #endregion Private Static Fields

        #region Constructors

        /// <summary>
        /// Initializes the wizard by setting up the default patterns.
        /// </summary>
        static Wizard()
        {
            Wizard._patterns = new Pattern[]
            {
                new Pattern(
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
                    0,
                    "image/png",
                    "png",
                    "Portable Network Graphics"),

                new Pattern(
                    new byte[] { 0xFF, 0xD8, 0xFF },
                    0,
                    "image/jpeg",
                    "jpg",
                    "Joint Photographic Experts Group"),

                new Pattern(
                    new byte[] { 0x47, 0x49, 0x46, 0x38 },
                    0,
                    "image/gif",
                    "gif",
                    "Graphics Interchange Format"),

                new Pattern(
                    new byte[] { 0x49, 0x49, 0x2A, 0x00 },
                    0,
                    "image/tiff",
                    "tiff",
                    "Tagged Image File Format")

                // FIXME: add like 5 million more.
            }.OrderByDescending(x => x.Signature.Length).ToList();
        }

        #endregion Constructors

        #region Public Static Methods

        /// <summary>
        /// Tries to identify the type of the file in the passed stream.
        /// Returns either the matched pattern or null if the file couldnt be
        /// identified.
        /// </summary>
        /// <remarks>
        /// THE STREAM HAS TO BE SEEKABLE!
        /// </remarks>
        /// <param name="stream">
        /// The stream from which the file should be identified.
        /// </param>
        /// <returns>
        /// Either the matched pattern instance or null.
        /// </returns>
        public static Pattern Identify(Stream stream)
        {
            long pos = stream.Position;

            foreach (Pattern pattern in Wizard._patterns)
            {
                try
                {
                    if (Wizard.DoesPatternMatchData(pattern, stream))
                    {
                        return pattern;
                    }
                }
                finally
                {
                    stream.Position = pos;
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to identify the type of the file in the passed buffer.
        /// Returns either the matched pattern or null if the file couldnt be
        /// identified.
        /// </summary>
        /// <remarks>
        /// THE STREAM HAS TO BE SEEKABLE!
        /// </remarks>
        /// <param name="buf">
        /// The buffer from which the file should be identified.
        /// </param>
        /// <returns>
        /// Either the matched pattern instance or null.
        /// </returns>
        public static Pattern Identify(byte[] buf)
        {
            using (Stream stream = new MemoryStream(buf))
            {
                return Wizard.Identify(stream);
            }
        }

        #endregion Public Static Methods

        #region Private Static Methods

        /// <summary>
        /// Checks whether the passed pattern matches the file in the passed
        /// stream.
        /// </summary>
        /// <param name="pattern">
        /// The pattern which should be checked for matching the file in the
        /// passed stream.
        /// </param>
        /// <param name="stream">
        /// The stream containing the file to be identified.
        /// </param>
        /// <returns>
        /// A flag indicating whether the passed pattern matches the data in
        /// the passed stream.
        /// </returns>
        private static bool DoesPatternMatchData(
            Pattern pattern, Stream stream)
        {
            if (stream.Length < pattern.Signature.Length + pattern.Offset)
            {
                return false;
            }

            stream.Seek(pattern.Offset, SeekOrigin.Current);
            byte[] buf = new byte[pattern.Signature.Length];
            stream.Read(buf, 0, buf.Length);

            return pattern.Signature.SequenceEqual(buf);
        }

        #endregion Private Static Methods
    }
}