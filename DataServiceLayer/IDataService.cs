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
    IList<ProductSearchModel> GetProductsByName(string search);
    IList<Product> GetProductsByCategory(int categoryId);


    /////////////////////////////////////////////////
    // Orders
    /////////////////////////////////////////////////

    Order? GetOrder(int id);
    IList<OrderSummaryModel> GetOrdersByShipName(string shipName);
    IList<OrderSummaryModel> GetAllOrders();

    /////////////////////////////////////////////////
    // Order Details
    /////////////////////////////////////////////////

    IList<OrderDetail> GetOrderDetailsByOrderId(int orderId);
    IList<OrderDetail> GetOrderDetailsByProductId(int productId);
}