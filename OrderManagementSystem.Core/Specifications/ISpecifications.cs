using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Specifications
{
    public interface ISpecifications <T> where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; set; } //Where 

        public List<Expression<Func<T, object>>> Includes { get; set; } //includes
    }
}
