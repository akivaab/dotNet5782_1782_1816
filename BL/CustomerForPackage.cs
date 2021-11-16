
namespace IBL
{
    namespace BO
    {
        public class CustomerForPackage
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public CustomerForPackage(int id, string name)
            {
                ID = id;
                Name = name;
            }
            public override string ToString()
            {
                return $"Package Customer: {ID}, {Name}";
            }
        }
    }
}