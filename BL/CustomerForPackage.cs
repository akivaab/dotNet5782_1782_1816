
namespace IBL
{
    namespace BO
    {
        public class CustomerForPackage
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public override string ToString()
            {
                return $"Package Customer: {ID}, {Name}";
            }
        }
    }
}