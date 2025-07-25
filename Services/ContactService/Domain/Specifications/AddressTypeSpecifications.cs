using BuildingBlocks.Domain.Specifications;
using ContactService.Domain.Aggregates.AddressType;
using ContactService.Domain.ValueObjects;
using System.Linq.Expressions;

namespace ContactService.Domain.Specifications;

public static class AddressTypeSpecifications
{
    public sealed class AllAddressTypesSpecification : Specification<AddressType>
    {
        public override Expression<Func<AddressType, bool>> ToExpression()
        {
            return type => true; // All types
        }
    }
    
    public sealed class AddressTypeByNameSpecification : Specification<AddressType>
    {
        private readonly AddressTypeName _name;
        
        public AddressTypeByNameSpecification(AddressTypeName name)
        {
            _name = name;
        }
        
        public override Expression<Func<AddressType, bool>> ToExpression()
        {
            return type => type.Name.Value == _name.Value;
        }
    }
    
    public sealed class ResidentialAddressTypesSpecification : Specification<AddressType>
    {
        public override Expression<Func<AddressType, bool>> ToExpression()
        {
            return type => type.Name.Value == AddressTypeName.Home.Value;
        }
    }
    
    public sealed class BusinessAddressTypesSpecification : Specification<AddressType>
    {
        public override Expression<Func<AddressType, bool>> ToExpression()
        {
            return type => type.Name.Value == AddressTypeName.Work.Value ||
                          type.Name.Value == AddressTypeName.Headquarters.Value ||
                          type.Name.Value == AddressTypeName.Warehouse.Value;
        }
    }
    
    public sealed class ShippingAddressTypesSpecification : Specification<AddressType>
    {
        public override Expression<Func<AddressType, bool>> ToExpression()
        {
            return type => type.Name.Value == AddressTypeName.Shipping.Value ||
                          type.Name.Value == AddressTypeName.Home.Value;
        }
    }
    
    public sealed class BillingAddressTypesSpecification : Specification<AddressType>
    {
        public override Expression<Func<AddressType, bool>> ToExpression()
        {
            return type => type.Name.Value == AddressTypeName.Billing.Value ||
                          type.Name.Value == AddressTypeName.Work.Value;
        }
    }
    
    public sealed class AddressTypesByNamePatternSpecification : Specification<AddressType>
    {
        private readonly string _namePattern;
        
        public AddressTypesByNamePatternSpecification(string namePattern)
        {
            _namePattern = namePattern;
        }
        
        public override Expression<Func<AddressType, bool>> ToExpression()
        {
            return type => type.Name.Value.Contains(_namePattern);
        }
    }
} 