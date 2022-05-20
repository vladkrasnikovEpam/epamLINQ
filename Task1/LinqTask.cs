using System;
using System.Collections.Generic;
using System.Linq;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            return customers.Where(x => x.Orders.Sum(x => x.Total) > limit).ToList();
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            var result = from Customer cust in customers
                         join sup in suppliers on cust.City equals sup.City into merged
                         select (cust, merged);

            return result;
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            var result = from cust in customers
                         group suppliers by cust into g
                         select (g.Key, g.SelectMany(x => x).Where(x => x.City == g.Key.City && x.Country == g.Key.Country));

            return result;
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            return customers.Where(x => x.Orders.Any(x => x.Total > limit)).ToList();
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            var result = from cust in customers
                         where cust.Orders.Count() > 0
                         select (cust, cust.Orders.Select(x => x.OrderDate).Min());
            return result;
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            var res = customers.Where(x => x.Orders.Count() > 0)
                .Select(x => (x, x.Orders.Select(x => x.OrderDate).Min()))
                .OrderBy(x => x.Item2.Year)
                .ThenBy(x => x.Item2.Month)
                .ThenByDescending(x => x.x.Orders.Select(x => x.Total).Sum())
                .ThenBy(x => x.x.CompanyName);

            return res;
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            var result = from cust in customers
                         where cust.PostalCode.Any(Char.IsLetter)
                        || cust.Region == null
                        || !cust.Phone.StartsWith('(')
                         select cust;

            return result;
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            /* example of Linq7result

             category - Beverages
	            UnitsInStock - 39
		            price - 18.0000
		            price - 19.0000
	            UnitsInStock - 17
		            price - 18.0000
		            price - 19.0000
             */
            var result = products.GroupBy(x => x.Category)
                .Select(x => new Linq7CategoryGroup()
                {
                    Category = x.Key,
                    UnitsInStockGroup = x.GroupBy(x => x.UnitsInStock)
                                            .Select(x => new Linq7UnitsInStockGroup()
                                            {
                                                UnitsInStock = x.Key,
                                                Prices = x.OrderBy(x => x.UnitPrice).Select(x => x.UnitPrice)
                                            })
                });
            return result;
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            var cheapQuery = from prod in products
                             where prod.UnitPrice > 0 && prod.UnitPrice <= cheap
                             select prod;

            var middlequery = from prod in products
                              where prod.UnitPrice > cheap && prod.UnitPrice <= middle
                              select prod;

            var expensiveQuery = from prod in products
                                 where prod.UnitPrice > middle && prod.UnitPrice <= expensive
                                 select prod;

            return new List<(decimal, IEnumerable<Product>)>
            {
                (cheap, cheapQuery),
                (middle, middlequery),
                (expensive, expensiveQuery)
            };
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            //var result = from cust in customers
            //             group customers by cust.City into g
            //             select (g.Key, 
            //             Convert.ToInt32(g.Select(x => x.Where(x => x.City == g.Key).Average(x => x.Orders.Sum(x => x.Total))).Average()), 
            //             (int)customers.Where(x => x.City == g.Key).Average(x => x.Orders.Count()));

            var alt = customers.GroupBy(x => x.City)
                .Select(x => (x.Key, 
                Convert.ToInt32(x.Average(x => x.Orders.Sum(x => x.Total))), 
                (int)x.Average(x => x.Orders.Count())));
            return alt;
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            var result = suppliers.Select(x => x.Country).Distinct().OrderBy(x => x.Length).ThenBy(x => x).ToArray();
            return string.Join("", result);
        }
    }
}