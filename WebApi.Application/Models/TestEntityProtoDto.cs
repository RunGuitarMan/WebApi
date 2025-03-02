using ProtoBuf;
using WebApi.Domain;

namespace WebApi.Application.Models;

[ProtoContract]
public class TestEntityProtoDto
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string? Content { get; set; }

    public TestEntityProtoDto() {}

    public TestEntityProtoDto(TestEntity entity)
    {
        Id = entity.Id;
        Content = entity.Content;
    }

    public TestEntity ToDomainModel()
    {
        return new TestEntity
        {
            Id = Id,
            Content = Content
        };
    }
}