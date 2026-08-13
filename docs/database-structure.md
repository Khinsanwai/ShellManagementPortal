# Database Structure Documentation

## Overview

| Property | Value |
|---|---|
| **Database** | `SMPortal` (SQL Server) |
| **ORM** | Entity Framework Core 8 (Code-First) |
| **Schemas** | `appshell` (core portal), `keycloak` (identity/tenant) |
| **DbContexts** | `AppDbContext` (read/write), `ReadDbContext` (read-only) |
| **Total Tables** | 11 |

---

## Schema: `appshell`

Core portal business data — applications, navigation, authorization, audit logs, and reference data.

---

### 1. `appshell.Applications`

**Purpose**: Registry of all child applications managed by the portal. Controls which applications appear in a user's dashboard sidebar.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `int` | NOT NULL | PK, Identity (auto-increment) |
| `Name` | `nvarchar(max)` | NOT NULL | Application display name |
| `Code` | `nvarchar(max)` | NOT NULL | Unique code identifier |
| `URL` | `nvarchar(max)` | NULL | Application URL |
| `Icon` | `nvarchar(max)` | NULL | Icon reference (CSS class or path) |
| `Description` | `nvarchar(max)` | NULL | Application description |
| `Version` | `nvarchar(max)` | NULL | Version string |
| `Status` | `bit` | NOT NULL | Active/inactive flag (default: `true`) |
| `DisplayOrder` | `int` | NOT NULL | Sort order (default: `0`) |
| `IsVisible` | `bit` | NOT NULL | Visibility in sidebar (default: `true`) |
| `CreatedDate` | `datetime2` | NOT NULL | Creation timestamp |
| `UpdatedDate` | `datetime2` | NULL | Last modification timestamp |

**Relationships**:
- Referenced by `UserChildApplicationAssignment.ApplicationId` (1:N)
- Referenced by `AppGroupAssignment.ApplicationId` (1:N)

---

### 2. `appshell.AppGroupAssignment`

**Purpose**: Maps WSO2 Identity Server groups to applications. When a group is assigned to an application, all users in that group gain access to it.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `uniqueidentifier` | NOT NULL | PK (GUID) |
| `ApplicationId` | `int` | NOT NULL | FK → `Applications.Id` (cascade delete), Indexed |
| `Wso2GroupId` | `nvarchar(max)` | NOT NULL | WSO2 IS group identifier |
| `Wso2GroupName` | `nvarchar(max)` | NOT NULL | WSO2 IS group display name |

**Relationships**:
- Many-to-One with `Applications` (cascade delete)

---

### 3. `appshell.UserChildApplicationAssignment`

**Purpose**: Per-user application assignment. Maps an individual WSO2 user to a specific child application for sidebar visibility.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `uniqueidentifier` | NOT NULL | PK (GUID) |
| `Wso2UserId` | `nvarchar(max)` | NOT NULL | WSO2 IS user identifier |
| `Wso2UserName` | `nvarchar(max)` | NOT NULL | WSO2 IS username |
| `ApplicationId` | `int` | NOT NULL | FK → `Applications.Id` (cascade delete), Indexed |

**Relationships**:
- Many-to-One with `Applications` (cascade delete)

---

### 4. `appshell.MenuItem`

**Purpose**: Hierarchical sidebar navigation menu structure. Supports parent-child relationships for grouped menus.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `uniqueidentifier` | NOT NULL | PK (GUID) |
| `Name` | `nvarchar(max)` | NOT NULL | Menu display text |
| `Url` | `nvarchar(max)` | NULL | Route URL (null for parent/group nodes) |
| `ParentId` | `uniqueidentifier` | NULL | Self-referencing parent (soft hierarchy, no FK) |
| `MenuOrder` | `int` | NULL | Sort order within parent |

**Menu Hierarchy** (from seed scripts):

```
├── Dashboard
├── Identity Management
│   ├── User List
│   ├── User Groups
│   ├── Roles
│   ├── Claims
│   └── Tenants
├── Organization Management
│   ├── Organization
│   └── Institutions
├── Application Management
│   └── Apps
├── Authorization Management
│   ├── Menu Items
│   ├── Resources
│   └── User Menu Assignment
└── System Management
    ├── Audit Dashboard
    ├── Audit Logs
    └── Settings
```

**Relationships**:
- Self-referencing via `ParentId` (no FK constraint)
- Referenced by `UserMenuAssignment.MenuItemId` (1:N)

---

### 5. `appshell.UserMenuAssignment`

**Purpose**: Controls sidebar menu visibility per user. Maps WSO2 users to specific menu items they are authorized to see.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `uniqueidentifier` | NOT NULL | PK (GUID) |
| `Wso2UserId` | `nvarchar(max)` | NOT NULL | WSO2 IS user identifier |
| `Wso2UserName` | `nvarchar(max)` | NOT NULL | WSO2 IS username |
| `MenuItemId` | `uniqueidentifier` | NOT NULL | FK → `MenuItem.Id` (cascade delete), Indexed |

**Relationships**:
- Many-to-One with `MenuItem` (cascade delete)

---

### 6. `appshell.Claim`

**Purpose**: Registry of claim types used for identity and authorization. Represents permission or attribute types.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `uniqueidentifier` | NOT NULL | PK (GUID) |
| `Name` | `nvarchar(max)` | NOT NULL | Claim type name |
| `Value` | `nvarchar(max)` | NULL | Claim value |
| `Description` | `nvarchar(max)` | NULL | Human-readable description |

**Relationships**: Standalone (no FK references).

---

### 7. `appshell.Institution`

**Purpose**: Registry of institutions (e.g., universities, organizations, business units) that use the portal.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `uniqueidentifier` | NOT NULL | PK (GUID) |
| `Name` | `nvarchar(max)` | NOT NULL | Institution name |
| `Code` | `nvarchar(max)` | NULL | Short institution code |
| `Description` | `nvarchar(max)` | NULL | Description |

**Relationships**: Standalone (no FK references).

---

### 8. `appshell.OrgUnit`

**Purpose**: Hierarchical organizational structure. Represents departments, divisions, or other organizational units.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `uniqueidentifier` | NOT NULL | PK (GUID) |
| `Name` | `nvarchar(max)` | NOT NULL | Organization unit name |
| `ParentUnitId` | `uniqueidentifier` | NULL | Self-referencing parent (soft hierarchy, no FK) |
| `Description` | `nvarchar(max)` | NULL | Description |

**Relationships**: Self-referencing via `ParentUnitId` (no FK constraint).

---

### 9. `appshell.Userlogs`

**Purpose**: Audit log table recording user activities across the portal. Tracks authentication events, administrative actions, and other operations.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `bigint` | NOT NULL | PK, Identity (auto-increment) |
| `UserId` | `nvarchar(max)` | NULL | WSO2 user ID of actor |
| `Username` | `nvarchar(max)` | NULL | Username of actor |
| `Category` | `nvarchar(max)` | NOT NULL | Log category (e.g., "Authentication") |
| `Action` | `nvarchar(max)` | NOT NULL | Action performed (e.g., "Login", "Create") |
| `Application` | `nvarchar(max)` | NULL | Application context |
| `Module` | `nvarchar(max)` | NULL | Module within the application |
| `Description` | `nvarchar(max)` | NULL | Human-readable event description |
| `Result` | `nvarchar(max)` | NOT NULL | Outcome (e.g., "Success", "Failure") |
| `IPAddress` | `nvarchar(max)` | NULL | Client IP address |
| `UserAgent` | `nvarchar(max)` | NULL | Browser/client user agent |
| `CreatedDate` | `datetime2` | NOT NULL | Event timestamp |

**Relationships**: Standalone. Append-only (rows are never updated or deleted).

---

## Schema: `keycloak`

Identity and tenant data aligned with WSO2/Keycloak concepts.

---

### 10. `keycloak.Tenant`

**Purpose**: Multi-tenant registry. Represents tenants (tenancy contexts) that can own resources.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `uniqueidentifier` | NOT NULL | PK (GUID) |
| `Name` | `nvarchar(max)` | NOT NULL | Tenant name |

**Relationships**: Referenced logically by `Resource.TenantId`.

---

### 11. `keycloak.Resource`

**Purpose**: Authorization resources. Links applications, tenants, and menu items to define what can be accessed.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| `Id` | `uniqueidentifier` | NOT NULL | PK (GUID) |
| `Name` | `nvarchar(max)` | NOT NULL | Resource name |
| `Description` | `nvarchar(max)` | NOT NULL | Resource description |
| `ApplicationId` | `uniqueidentifier` | NOT NULL | Logical FK to application (no DB constraint) |
| `TenantId` | `uniqueidentifier` | NOT NULL | Logical FK to tenant (no DB constraint) |
| `MenuItemId` | `uniqueidentifier` | NULL | Logical FK to menu item (no DB constraint) |

**Relationships**: No enforced FK constraints. All references are logical/conceptual.

---

## Entity Relationship Diagram

```
┌─────────────────────┐
│    Applications      │
│  (appshell.Apps)     │
│  PK: Id (int, ID)    │
└──────────┬──────────┘
           │ 1
           │
     ┌─────┴──────┐
     │            │
     N            N
┌────┴─────┐  ┌───┴──────────────────┐
│ AppGroup │  │ UserChildApplication │
│Assignment│  │    Assignment        │
│(GroupId) │  │  (Wso2UserId)        │
└──────────┘  └──────────────────────┘

┌─────────────────────┐
│     MenuItem         │
│  (appshell)          │
│  PK: Id (GUID)       │
│  ParentId → MenuItem │ (self-ref)
└──────────┬──────────┘
           │ 1
           │
           N
┌─────────────────────┐
│ UserMenuAssignment   │
│  (Wso2UserId)        │
│  FK: MenuItemId      │
└─────────────────────┘

┌─────────────────────┐       ┌──────────────────┐
│     Tenant           │       │    OrgUnit        │
│  (keycloak)          │       │  (appshell)       │
│  PK: Id (GUID)       │       │  PK: Id (GUID)    │
└──────────┬──────────┘       │  ParentUnitId     │
           │                   └──────────────────┘
           N
┌─────────────────────┐
│     Resource         │
│  (keycloak)          │
│  PK: Id (GUID)       │
│  AppId, TenantId,    │
│  MenuItemId          │
└─────────────────────┘

┌─────────────────────┐  ┌──────────────────┐  ┌──────────────┐
│     Claim            │  │   Institution    │  │   Userlogs   │
│  (appshell)          │  │  (appshell)      │  │ (appshell)   │
│  Standalone          │  │  Standalone      │  │  Standalone  │
└─────────────────────┘  └──────────────────┘  └──────────────┘
```

---

## Dual DbContext Pattern

| Context | File | Purpose |
|---|---|---|
| `AppDbContext` | `ApplicationDbContext/AppDbContext.cs` | Read/Write — exposes all 11 DbSets |
| `ReadDbContext` | `ApplicationDbContext/ReadDbContext.cs` | Read-only — exposes 10 DbSets (excludes `AppGroupAssignment`). Overrides `SaveChanges()` to throw `InvalidOperationException` |

---

## External Entities (Not in Database)

The following are managed externally by WSO2 Identity Server via SCIM 2.0 API:

| Entity | Purpose |
|---|---|
| **Users** | WSO2 IS user accounts (SCIM `/Users`) |
| **Groups** | WSO2 IS groups for role-based access (SCIM `/Groups`) |
| **Roles** | WSO2 IS roles with permissions (SCIM `/Roles`) |

These have no local database tables — the app communicates with WSO2 via `ScimService.cs` and `Wso2Service.cs`.

---

## Migration History

| Migration | Date | Changes |
|---|---|---|
| `InitialCreate` | 2026-07-10 | Created schemas `appshell` and `keycloak`. Tables: App, Claim, Institution, MenuItem, OrgUnit, Resource, Tenant |
| `AddUserMenuAssignment` | 2026-07-10 | Created `UserMenuAssignment` with FK to `MenuItem` |
| `AddUserChildAppAssignment` | 2026-07-24 | Added `UserChildApplicationAssignment` |
| `AddAppDisplayOrderAndIsVisible` | 2026-07-24 | Added `DisplayOrder` and `IsVisible` to `Applications` |
| `AddAppGroupAssignment` | 2026-08-04 | Created `AppGroupAssignment` with FK to `Applications` |
