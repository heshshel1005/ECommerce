using System;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace ECommerce.Catalog;

public class AttributeDefinitionGovernanceTests
{
    [Fact]
    public void Should_Start_As_Draft_With_Version_Zero()
    {
        var d = new AttributeDefinition(Guid.NewGuid(), "k", AttributeDefinitionDataType.Text);
        d.GovernanceStatus.ShouldBe(AttributeDefinitionGovernanceStatus.Draft);
        d.PublishedVersion.ShouldBe(0);
    }

    [Fact]
    public void Should_Submit_For_Review_From_Draft()
    {
        var d = new AttributeDefinition(Guid.NewGuid(), "k", AttributeDefinitionDataType.Text);
        d.SubmitForReview();
        d.GovernanceStatus.ShouldBe(AttributeDefinitionGovernanceStatus.PendingReview);
    }

    [Fact]
    public void Should_Reject_Review_To_Draft()
    {
        var d = new AttributeDefinition(Guid.NewGuid(), "k", AttributeDefinitionDataType.Text);
        d.SubmitForReview();
        d.RejectReview();
        d.GovernanceStatus.ShouldBe(AttributeDefinitionGovernanceStatus.Draft);
    }

    [Fact]
    public void Should_Publish_And_Increment_Version()
    {
        var d = new AttributeDefinition(Guid.NewGuid(), "k", AttributeDefinitionDataType.Text);
        d.Publish();
        d.GovernanceStatus.ShouldBe(AttributeDefinitionGovernanceStatus.Published);
        d.PublishedVersion.ShouldBe(1);
        d.Publish();
        d.PublishedVersion.ShouldBe(2);
    }

    [Fact]
    public void Should_Block_Archived_Mutation_On_Submit()
    {
        var d = new AttributeDefinition(Guid.NewGuid(), "k", AttributeDefinitionDataType.Text);
        d.Publish();
        d.Archive();
        Should.Throw<BusinessException>(() => d.SubmitForReview());
    }
}
