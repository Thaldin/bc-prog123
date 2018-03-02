using System.Collections.Generic;

namespace Abshire_Ed.Models
{
    public class ProductList
    {
        LinkedList<ProductModel> _products;

        public LinkedList<ProductModel> Products
        {
            get
            {
                if (_products == null)
                    _products = new LinkedList<ProductModel>();
                return _products;
            }
        }
    }
}
