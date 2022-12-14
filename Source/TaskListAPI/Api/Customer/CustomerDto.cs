using System.ComponentModel;

namespace TaskListAPI
{
    public class CustomerDto : EditCustomerDto
    {
        [ReadOnly(true)]
        public int Id { get; set; }
    }
}