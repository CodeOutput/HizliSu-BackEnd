using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace HizliSu.General
{

    [AutoMapFrom(typeof(File))]
    public class FileDto : FullAuditedEntityDto<long>
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
