namespace TaskListAPI
{
    public class CustomerConverter
    {
        public CustomerDto ModelToDto(CustomerModel model)
        {
            return new CustomerDto
            {
                Id = model.Id,
                Name = model.Name
            };
        }

        public void DtoToModel(EditCustomerDto dto, CustomerModel model, int id)
        {
            model.Id = id;
            model.Name = dto.Name;
        }
    }
}