namespace RabitMqTest;

public record CustomerData
{
    
    public int UserId { get; set; }
    public List<CustomerDataProperty>? Properties { get; set; }
}

