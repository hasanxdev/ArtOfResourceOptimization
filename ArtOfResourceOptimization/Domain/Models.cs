using ProtoBuf;

namespace ArtOfResourceOptimization.Domain;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string StoreName { get; set; }
    public int StoreId { get; set; }
    public string Category { get; set; }
    public string Barcode { get; set; }
}

public class MainProduct : Product
{
    public string Description { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Brand { get; set; }
    public string StoreAddress { get; set; }
}

[ProtoContract]
public class CachedProduct
{
    [ProtoMember(1)]
    public string Name { get; set; }
    [ProtoMember(2)]
    public string Category { get; set; }
    [ProtoMember(3)]
    public string Barcode { get; set; }
}

[ProtoContract]
public class CachedProductStore
{
    [ProtoMember(1)]
    public decimal Price { get; set; }
    [ProtoMember(2)]
    public int Quantity { get; set; }
    [ProtoMember(3)]
    public string StoreName { get; set; }
    [ProtoMember(4)]
    public int StoreId { get; set; }
}