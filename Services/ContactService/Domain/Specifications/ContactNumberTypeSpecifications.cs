using BuildingBlocks.Domain.Specifications;
using ContactService.Domain.Aggregates.ContactNumberType;
using ContactService.Domain.ValueObjects;
using System.Linq.Expressions;

namespace ContactService.Domain.Specifications;

public static class ContactNumberTypeSpecifications
{
    public sealed class AllContactNumberTypesSpecification : Specification<ContactNumberType>
    {
        public override Expression<Func<ContactNumberType, bool>> ToExpression()
        {
            return type => true; // All types
        }
    }
    
    public sealed class ContactNumberTypeByNameSpecification : Specification<ContactNumberType>
    {
        private readonly ContactNumberTypeName _name;
        
        public ContactNumberTypeByNameSpecification(ContactNumberTypeName name)
        {
            _name = name;
        }
        
        public override Expression<Func<ContactNumberType, bool>> ToExpression()
        {
            return type => type.Name.Value == _name.Value;
        }
    }
    
    public sealed class DefaultContactNumberTypesSpecification : Specification<ContactNumberType>
    {
        public override Expression<Func<ContactNumberType, bool>> ToExpression()
        {
            return type => type.Name.Value == ContactNumberTypeName.Mobile.Value ||
                          type.Name.Value == ContactNumberTypeName.Home.Value;
        }
    }
    
    public sealed class WorkRelatedContactNumberTypesSpecification : Specification<ContactNumberType>
    {
        public override Expression<Func<ContactNumberType, bool>> ToExpression()
        {
            return type => type.Name.Value == ContactNumberTypeName.Work.Value ||
                          type.Name.Value == ContactNumberTypeName.Fax.Value;
        }
    }
    
    public sealed class EmergencyContactNumberTypesSpecification : Specification<ContactNumberType>
    {
        public override Expression<Func<ContactNumberType, bool>> ToExpression()
        {
            return type => type.Name.Value == ContactNumberTypeName.Emergency.Value;
        }
    }
    
    public sealed class ContactNumberTypesByNamePatternSpecification : Specification<ContactNumberType>
    {
        private readonly string _namePattern;
        
        public ContactNumberTypesByNamePatternSpecification(string namePattern)
        {
            _namePattern = namePattern;
        }
        
        public override Expression<Func<ContactNumberType, bool>> ToExpression()
        {
            return type => type.Name.Value.Contains(_namePattern);
        }
    }
} 