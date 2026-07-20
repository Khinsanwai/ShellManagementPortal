# Plan: Integrate WSO2 API Manager into Shell Management Portal

## Context

The Shell Management Portal currently uses **WSO2 Identity Server** for authentication (OIDC) and user/group management (SCIM 2.0). The API is consumed directly by the Blazor Web frontend — there is no API gateway, rate limiting, API key management, or developer portal.

Integrating **WSO2 API Manager (APIM)** adds a management layer in front of the existing API, providing gateway proxying, throttling, API lifecycle management, a developer portal for API consumers, and usage analytics.

## Current Architecture

```
Blazor Web (port 7067) ──direct──> ShellMgmt.Api (port 7203) ──> SQL Server
       │                                    │
       └── OIDC ──> WSO2 IS (port 9443)     └── SCIM ──> WSO2 IS
```

## Target Architecture

```
Blazor Web (port 7067) ──> WSO2 API Manager Gateway ──> ShellMgmt.Api (port 7203) ──> SQL Server
       │                            │
       └── OIDC ──> WSO2 IS         └── Token validation, Throttling, Analytics
                                          │
                                     Publisher Portal (API lifecycle)
                                     Developer Portal (API discovery + subscription)
```

---

## Feature Extension Phases

### Phase 1: API Manager Core Setup

**Goal**: Get WSO2 APIM running and proxying the existing API.

1. **Install & Configure WSO2 API Manager 4.x**
   - Download from [WSO2 APIM](https://wso2.com/api-manager/)
   - Configure to use the existing WSO2 IS as the Key Manager (shared identity)
   - Default ports: Publisher `9443`, Gateway `8243`/`8280`

2. **Import Existing APIs into Publisher**
   - The project already has Swashbuckle/Swagger at `https://localhost:7203/swagger`
   - Import the OpenAPI spec into WSO2 APIM Publisher for each controller:
     - `Claim`, `Institution`, `MenuItem`, `OrgUnit`, `Resource`, `Tenant`, `App`
     - `User`, `Group` (SCIM-based)
     - `UserMenu`
   - Set backend endpoint to `https://localhost:7203/api/v1/{controller}`

3. **Publish APIs**
   - Move each API through lifecycle: Created → Published
   - APIs appear in the Developer Portal

4. **Update Web Project to Call via Gateway**
   - Change `SMPConfig:Url` in `Web/appsettings.json` from `https://localhost:7203/api/v1/{0}` to the APIM gateway URL (e.g., `https://localhost:8243/api/v1/{0}`)
   - The Web app subscribes to APIs via Developer Portal and uses a gateway token

**Files to modify**:
- `Web/appsettings.json` — update API URL to gateway endpoint
- `Web/Services/ApiService.cs` — may need to pass API key or gateway token in headers

---

### Phase 2: API Security & Throttling

**Goal**: Add rate limiting, API key auth, and security policies.

1. **Define Throttling Policies** (in Publisher)
   - `Unlimited` — for admin/internal use
   - `Gold/Silver/Bronze` — tiered rate limits (e.g., 50/20/5 requests per minute)
   - Apply per-API or per-resource throttling

2. **Security Schemes**
   - Keep JWT/OAuth2 for the Web frontend (same WSO2 IS tokens)
   - Add API Key support for external consumers
   - Configure OAuth2 scopes per API resource for fine-grained access

3. **Add API Key Support to the API Project** (optional, for external consumers)
   - Create middleware in `ShellMgmt.Api` to validate `apikey` header from the gateway
   - Or rely on APIM gateway to handle this transparently

**Files to create/modify**:
- `Api/ShellMgmt.Api/Middleware/ApiKeyMiddleware.cs` (optional)
- `Api/ShellMgmt.Api/Program.cs` — register middleware
- APIM Publisher — configure policies per API

---

### Phase 3: Developer Portal Integration

**Goal**: Enable self-service API discovery and subscription.

1. **Register Developer Portal Application**
   - Create an application in the Developer Portal
   - Generate consumer key/secret for the Web app
   - Subscribe the Web app to all required APIs

2. **Add Developer Portal Link to Blazor Web**
   - Add a menu item or link in `MainLayout.razor` pointing to the Developer Portal
   - Admin users can manage API subscriptions from the portal

3. **API Documentation**
   - APIM auto-generates interactive docs from OpenAPI specs
   - Add custom API docs in the Publisher for onboarding

**Files to modify**:
- `Web/Components/Layout/MainLayout.razor` — add Developer Portal link
- `database/seed_all_menus.sql` — add Developer Portal menu item
- `Web/appsettings.json` — add Developer Portal URL config

---

### Phase 4: API Lifecycle Management

**Goal**: Manage API versions, deprecation, and retirement.

1. **API Versioning in APIM**
   - Map existing `api/v1/` routes to APIM API versions
   - When releasing v2, create a new API version in Publisher
   - Route traffic: v1 consumers continue on v1, new consumers get v2

2. **Lifecycle States**
   - `Created` → `Published` → `Deprecated` → `Retired`
   - Deprecation notice appears in Developer Portal
   - Retired APIs return `410 Gone`

3. **Add Lifecycle Management UI to Blazor** (optional, advanced)
   - New page: `ApiLifecycle.razor` showing API status from Publisher API
   - Uses Publisher REST API: `GET /api/am/publisher/v4/apis`
   - Requires Publisher API OAuth token

**Files to create**:
- `Web/Components/Pages/ApiLifecycle/ApiLifecycleList.razor`
- `Web/Services/PublisherApiService.cs` — calls APIM Publisher REST API
- `Api/ShellMgmt.Domain/ApiModels/` — API metadata DTOs

---

### Phase 5: Analytics & Monitoring

**Goal**: Track API usage, errors, and performance.

1. **Enable APIM Analytics**
   - Configure WSO2 APIM analytics component
   - Tracks: request count, latency, error rates, top consumers

2. **Add Analytics Dashboard to Blazor** (optional)
   - New page: `ApiAnalytics.razor`
   - Calls APIM Analytics REST API for metrics
   - Display charts using Radzen chart components

3. **API Health Monitoring**
   - Add health check endpoint to `ShellMgmt.Api`
   - APIM can monitor backend health

**Files to create**:
- `Api/ShellMgmt.Api/Controllers/Health/HealthController.cs`
- `Web/Components/Pages/Analytics/ApiAnalytics.razor`
- `Web/Services/AnalyticsService.cs`

---

### Phase 6: Advanced Features (Future)

These are optional extensions after the core integration is working:

1. **API Products** — Bundle multiple APIs into a single product for consumers
2. **Monetization** — Set up billing tiers for API usage
3. **Choreo Connect** — Deploy a microgateway for containerized scenarios
4. **GraphQL Support** — If the API grows, add GraphQL endpoints managed by APIM
5. **AsyncAPI** — For event-driven features (SignalR, webhooks)

---

## Implementation Priority

| Phase | Effort | Value | Priority |
|-------|--------|-------|----------|
| Phase 1: Core Setup | Medium | High | **Start here** |
| Phase 2: Security & Throttling | Medium | High | **Next** |
| Phase 3: Developer Portal | Low | Medium | After Phase 2 |
| Phase 4: Lifecycle Management | Medium | Medium | When versioning APIs |
| Phase 5: Analytics | Low | Medium | When in production |
| Phase 6: Advanced | High | Low | Future |

---

## Prerequisites

- WSO2 API Manager 4.x installed and running
- Existing WSO2 IS configured as the Key Manager for APIM
- SQL Server running with `SMPortal` database
- Both API and Web projects building successfully

## Key Configuration Changes

### Web/appsettings.json — Add APIM section
```json
{
  "WSO2": {
    "//": "existing WSO2 IS config stays"
  },
  "SMPConfig": {
    "Url": "https://localhost:8243/api/v1/{0}",
    "IamUrl": "http://localhost:5107/api/v1/{0}",
    "BaseRouteUrl": "/"
  },
  "WSO2APIM": {
    "GatewayUrl": "https://localhost:8243",
    "PublisherUrl": "https://localhost:9443/api/am/publisher/v4",
    "DeveloperPortalUrl": "https://localhost:9443/devportal",
    "ConsumerKey": "",
    "ConsumerSecret": ""
  }
}
```

## Verification

After each phase:
1. Build: `dotnet build ShellManagementPortal.sln`
2. Run both projects and verify the Web app still functions
3. Check Swagger at `https://localhost:7203/swagger` still works (direct access)
4. Verify gateway proxy: call API through APIM gateway URL
5. Check APIM Publisher/Developer Portal dashboards

## References

- [WSO2 APIM Documentation](https://apim.docs.wso2.com/en/latest/)
- [WSO2 APIM GitHub](https://github.com/wso2/product-apim)
- [Publisher REST API](https://apim.docs.wso2.com/en/latest/reference/product-apis/publisher-apis/publisher-v4/publisher-v4/)
- [Developer Portal REST API](https://apim.docs.wso2.com/en/latest/reference/product-apis/devportal-apis/devportal-v4/devportal-v4/)
