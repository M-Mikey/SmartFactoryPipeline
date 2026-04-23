using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MachineData
{
    [Table("MachineInputs")]
    public class MachineInputs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string machineId { get; set; }
        public double temperature { get; set; }
        public DateTime timestamp { get; set; }
    }
}