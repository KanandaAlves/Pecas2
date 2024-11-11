using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Pecas2.Models
{
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A data do pedido é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        [DisplayName("Cliente")]
        public int ClienteId { get; set; }

        public Cliente? Cliente { get; set; }

        public ICollection<ItemPedido>? ItemPedidos { get; set; } = new List<ItemPedido>();
    }
}
