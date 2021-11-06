
namespace IBL
{
    namespace BO
    {
        public class CustomerForPackage
        {
            public int ID { get; }
            public string Name { get; }
            public override string ToString()
            {
                return $"Package Customer: {ID}, {Name}";
            }
        }
    }
}