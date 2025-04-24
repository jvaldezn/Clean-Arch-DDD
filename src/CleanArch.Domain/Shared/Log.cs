using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Domain.Shared
{
    [Table("log")]
    public class Log
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("hostname")]
        public string? MachineName { get; set; }

        [Column("logged")]
        public DateTime Logged { get; set; }

        [Column("level")]
        public string? Level { get; set; }

        [Column("message")]
        public string? Message { get; set; }

        [Column("logger")]
        public string? Logger { get; set; }

        [Column("properties")]
        public string? Properties { get; set; }

        [Column("callsite")]
        public string? Callsite { get; set; }

        [Column("exception")]
        public string? Exception { get; set; }

        [Column("applicationId")]
        public int ApplicationId { get; set; }
    }
}
