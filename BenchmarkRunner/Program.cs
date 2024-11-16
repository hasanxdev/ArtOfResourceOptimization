// See https://aka.ms/new-console-template for more information

using System.IO.Compression;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using MessagePack;
using Newtonsoft.Json;
using ProtoBuf;
using JsonSerializer = System.Text.Json.JsonSerializer;

var summary = BenchmarkRunner.Run<DeserializerBenchmark>();

[Config(typeof(OutputSizeConfig))]
public class DeserializerBenchmark
{
    public ProductForBenchamrk Product { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        Product = new ProductForBenchamrk()
        {
            Id = int.MaxValue,
            Barcode = "12345678",
            StoreName = "Store name For this product",
            StoreId = 453942920,
            Price = 125123132,
            Quantity = 550,
            Category = "Sample category",
            Name = "This is a test product with random values just generated",
        };
    }

    [Benchmark]
    public byte[] SystemTextJson()
    {
        return Compress(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Product)));
    }
    
    [Benchmark]
    public byte[] NewtonsoftJson()
    {
        return Compress(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Product)));
    }

    [Benchmark]
    public byte[] Protobuf()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, Product);
        return ms.ToArray();
    }
    
    [Benchmark]
    public byte[] ProtobufWithCompression()
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, Product);
        return Compress(ms.ToArray());
    }
    
    [Benchmark]
    public byte[] MessagePack()
    {
        return MessagePackSerializer.Serialize(Product);
    }
    
    [Benchmark]
    public byte[] MessagePackWithCompression()
    {
        return Compress(MessagePackSerializer.Serialize(Product));
    }

    public byte[] Compress(byte[] bytes)
    {
        using var compressionStream = new MemoryStream();
        byte[] bytesLength = BitConverter.GetBytes(bytes.Length);
        compressionStream.Write(bytesLength, 0, 4);

        using (var gZipCompressionStream = new GZipStream(compressionStream, CompressionMode.Compress))
        {
            gZipCompressionStream.Write(bytes, 0, bytes.Length);
            gZipCompressionStream.Flush();
        }

        return compressionStream.ToArray();
    }
}

public class OutputSizeConfig : ManualConfig
{
    public OutputSizeConfig()
    {
        Add(new TagColumn("NewtonsoftJson",
            _ =>
            {
                var benchmark = new DeserializerBenchmark();
                benchmark.Setup();

                return $"{benchmark.NewtonsoftJson().Length} bytes";
            }));
        
        Add(new TagColumn("SystemTextJson",
            _ =>
            {
                var benchmark = new DeserializerBenchmark();
                benchmark.Setup();

                return $"{benchmark.SystemTextJson().Length} bytes";
            }));
        
        Add(new TagColumn("Protobuf",
            _ =>
            {
                var benchmark = new DeserializerBenchmark();
                benchmark.Setup();

                return $"{benchmark.Protobuf().Length} bytes";
            }));
        
        Add(new TagColumn("ProtobufWithCompression",
            _ =>
            {
                var benchmark = new DeserializerBenchmark();
                benchmark.Setup();

                return $"{benchmark.ProtobufWithCompression().Length} bytes";
            }));
        
        Add(new TagColumn("MessagePack",
            _ =>
            {
                var benchmark = new DeserializerBenchmark();
                benchmark.Setup();

                return $"{benchmark.MessagePack().Length} bytes";
            }));
        
        Add(new TagColumn("MessagePackWithCompression",
            test =>
            {
                var benchmark = new DeserializerBenchmark();
                benchmark.Setup();

                return $"{benchmark.MessagePackWithCompression().Length} bytes";
            }));
    }
}

[ProtoContract]
[MessagePackObject]
public class ProductForBenchamrk
{
    [ProtoMember(1)]
    [Key(0)]
    public int Id { get; set; }
    [ProtoMember(2)]
    [Key(1)]
    public string Name { get; set; }
    [ProtoMember(3)]
    [Key(2)]
    public decimal Price { get; set; }
    [ProtoMember(4)]
    [Key(3)]
    public int Quantity { get; set; }
    [ProtoMember(5)]
    [Key(4)]
    public string StoreName { get; set; }
    [ProtoMember(6)]
    [Key(5)]
    public int StoreId { get; set; }
    [ProtoMember(7)]
    [Key(6)]
    public string Category { get; set; }
    [ProtoMember(8)]
    [Key(7)]
    public string Barcode { get; set; }
}

// | Method                     | Mean       | Error     | StdDev    | NewtonsoftJson | SystemTextJson | Protobuf  | ProtobufWithCompression | MessagePack | MessagePackWithCompression |
// |--------------------------- |-----------:|----------:|----------:|--------------- |--------------- |---------- |------------------------ |------------ |--------------------------- |
// | SystemTextJson             | 8,196.8 ns | 140.12 ns | 330.28 ns | 203 bytes      | 201 bytes      | 136 bytes | 155 bytes               | 135 bytes   | 155 bytes                  |
// | NewtonsoftJson             | 8,521.9 ns | 142.87 ns | 133.64 ns | 203 bytes      | 201 bytes      | 136 bytes | 155 bytes               | 135 bytes   | 155 bytes                  |
// | Protobuf                   |   245.5 ns |   4.79 ns |   4.48 ns | 203 bytes      | 201 bytes      | 136 bytes | 155 bytes               | 135 bytes   | 155 bytes                  |
// | ProtobufWithCompression    | 7,120.9 ns |  66.05 ns |  58.55 ns | 203 bytes      | 201 bytes      | 136 bytes | 155 bytes               | 135 bytes   | 155 bytes                  |
// | MessagePack                |   154.9 ns |   3.06 ns |   3.15 ns | 203 bytes      | 201 bytes      | 136 bytes | 155 bytes               | 135 bytes   | 155 bytes                  |
// | MessagePackWithCompression | 6,657.9 ns |  23.62 ns |  22.10 ns | 203 bytes      | 201 bytes      | 136 bytes | 155 bytes               | 135 bytes   | 155 bytes                  |
