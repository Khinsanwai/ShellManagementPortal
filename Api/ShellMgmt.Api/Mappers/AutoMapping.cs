using AutoMapper;
using SharedKernel.Domain;
using ShellMgmt.Application.AppService.Get;
using ShellMgmt.Application.ClaimService.Create;
using ShellMgmt.Application.ClaimService.Get;
using ShellMgmt.Application.InstitutionService.Create;
using ShellMgmt.Application.InstitutionService.Get;
using ShellMgmt.Application.MenuItemService.Get;
using ShellMgmt.Application.OrgUnitService.Create;
using ShellMgmt.Application.OrgUnitService.Get;
using ShellMgmt.Application.ResourceService.Get;
using ShellMgmt.Application.TenantService.GetByName;
using ShellMgmt.Domain.AppModels;
using ShellMgmt.Domain.ClaimModels;
using ShellMgmt.Domain.InstitutionModels;
using ShellMgmt.Domain.MenuItemModels;
using ShellMgmt.Domain.OrgUnitModels;
using ShellMgmt.Domain.ResourceModels;
using ShellMgmt.Domain.TenantModels;

namespace ShellMgmt.Api.Mappers;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        // MenuItem
        CreateMap<MenuItem, MenuItemDto>();
        CreateMap<PagedList<MenuItem>, PagedList<MenuItemDto>>().ReverseMap();
        CreateMap<GetMenuItemQuery, MenuItemDto>().ReverseMap();

        // Tenant
        CreateMap<Tenant, TenantDto>();
        CreateMap<GetTenantByNameQuery, TenantDto>().ReverseMap();

        // Resource
        CreateMap<Resource, ResourceDto>();
        CreateMap<PagedList<Resource>, PagedList<ResourceDto>>().ReverseMap();
        CreateMap<GetResourceQuery, ResourceDto>().ReverseMap();

        // Claim
        CreateMap<Claim, ClaimDto>();
        CreateMap<PagedList<Claim>, PagedList<ClaimDto>>().ReverseMap();
        CreateMap<GetClaimQuery, ClaimDto>().ReverseMap();
        CreateMap<CreateClaimCommand, ClaimDto>().ReverseMap();

        // Institution
        CreateMap<Institution, InstitutionDto>();
        CreateMap<PagedList<Institution>, PagedList<InstitutionDto>>().ReverseMap();
        CreateMap<GetInstitutionQuery, InstitutionDto>().ReverseMap();
        CreateMap<CreateInstitutionCommand, InstitutionDto>().ReverseMap();

        // OrgUnit
        CreateMap<OrgUnit, OrgUnitDto>();
        CreateMap<PagedList<OrgUnit>, PagedList<OrgUnitDto>>().ReverseMap();
        CreateMap<GetOrgUnitQuery, OrgUnitDto>().ReverseMap();
        CreateMap<CreateOrgUnitCommand, OrgUnitDto>().ReverseMap();

        // App
        CreateMap<App, AppDto>();
        CreateMap<PagedList<App>, PagedList<AppDto>>().ReverseMap();
        CreateMap<GetAppQuery, AppDto>().ReverseMap();
    }
}
