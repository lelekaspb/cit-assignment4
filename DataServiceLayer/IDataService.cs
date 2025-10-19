namespace DataServiceLayer;
public interface IDataService
{

    /////////////////////////////////////////////////
    // Categories
    /////////////////////////////////////////////////

    int GetCategoriesCount();
    IList<Category> GetCategories();
    Category? GetCategory(int id);
    Category CreateCategory(string name, string description);
    bool UpdateCategory(int id, string name, string description);
    bool DeleteCategory(int id);


    /////////////////////////////////////////////////
    // Products
    /////////////////////////////////////////////////

    Product? GetProduct(int id);
    IList<ProductSearchModel> GetProductByName(string search);
    IList<Product> GetProductByCategory(int categoryId);


    /////////////////////////////////////////////////
    // Orders
    /////////////////////////////////////////////////

    Order? GetOrder(int id);
    IList<OrderSummaryModel> GetOrdersByShipName(string shipName);
    IList<OrderSummaryModel> GetOrders();

    /////////////////////////////////////////////////
    // Order Details
    /////////////////////////////////////////////////

    IList<OrderDetail> GetOrderDetailsByOrderId(int orderId);
    IList<OrderDetail> GetOrderDetailsByProductId(int productId);
}