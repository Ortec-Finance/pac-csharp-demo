using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace TaskListAPI
{
    [Table("customers")]
    public class CustomerModel
    {
        [Key]
        [DataMember]
        [Column("id")]
        public int Id { get; set; }

        [DataMember]
        [Column("name")]
        [MaxLength(256)]
        public string Name { get; set; } = null!;
    }
}