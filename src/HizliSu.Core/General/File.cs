using System;
using Abp.Domain.Entities.Auditing;

namespace HizliSu.General
{
    public class File : FullAuditedEntity<long>
    {

        /// <summary>
        /// request isteğinde yollanan dosya adıdır
        /// </summary>
        public string UserFileName { get; set; }

        /// <summary>
        /// Content içindeki FileName
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// application/pdf, image/jpeg gibi content type bilgisi
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Dosya boyutu
        /// </summary>
        public long Length { get; set; }


        /// <summary>
        /// Dosyanın kendisi
        /// </summary>
        public byte[] Content { get; set; }
    }
}
