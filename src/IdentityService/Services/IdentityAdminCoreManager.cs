
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using AutoMapper;
using IdentityAdmin.Core;
using IdentityAdmin.Core.Client;
using IdentityAdmin.Core.Metadata;
using IdentityAdmin.Core.Scope;
using IdentityServer3.Core.Models;
using IdentityServer3.EntityFrameworkCore.DbContexts;
using IdentityServer3.EntityFrameworkCore.Entities;
using Client = IdentityServer3.EntityFrameworkCore.Entities.Client;
using Scope = IdentityServer3.EntityFrameworkCore.Entities.Scope;
using ScopeClaim = IdentityServer3.EntityFrameworkCore.Entities.ScopeClaim;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using IdentityAdmin.Extensions;

namespace IdentityServer3.Admin.EntityFrameworkCore
{
    public class IdentityAdminCoreManager<TCLient, TScope, TScopeConfigurationContext, TClientConfigurationContext> : IIdentityAdminService
        where TCLient : Client,new()
        where TScope : Scope,new()
        where TScopeConfigurationContext: ScopeConfigurationContext
        where TClientConfigurationContext: ClientConfigurationContext
    {
        private static IMapper _clientMapper;

        private readonly TScopeConfigurationContext _scopeContext;
        private readonly TClientConfigurationContext _clientContext;
        public IdentityAdminCoreManager(TScopeConfigurationContext scopeContext, TClientConfigurationContext clientContext)
        {
            this._scopeContext = scopeContext;
            this._clientContext = clientContext;


            var clientConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TCLient, Client>();
                cfg.CreateMap<Client, TCLient>();
                cfg.CreateMap<ClientClaim, ClientClaimValue>();
                cfg.CreateMap<ClientClaimValue, ClientClaim>();
                cfg.CreateMap<ClientSecret, ClientSecretValue>();
                cfg.CreateMap<ClientSecretValue, ClientSecret>();
                cfg.CreateMap<ClientIdPRestriction, ClientIdPRestrictionValue>();
                cfg.CreateMap<ClientIdPRestrictionValue, ClientIdPRestriction>();
                cfg.CreateMap<ClientPostLogoutRedirectUri, ClientPostLogoutRedirectUriValue>();
                cfg.CreateMap<ClientPostLogoutRedirectUriValue, ClientPostLogoutRedirectUri>();
                cfg.CreateMap<ClientRedirectUri, ClientRedirectUriValue>();
                cfg.CreateMap<ClientRedirectUriValue, ClientRedirectUri>();
                cfg.CreateMap<ClientCorsOrigin, ClientCorsOriginValue>();
                cfg.CreateMap<ClientCorsOriginValue, ClientCorsOrigin>();
                cfg.CreateMap<ClientCustomGrantType, ClientCustomGrantTypeValue>();
                cfg.CreateMap<ClientCustomGrantTypeValue, ClientCustomGrantType>();
                cfg.CreateMap<ClientScope, ClientScopeValue>();
                cfg.CreateMap<ClientScopeValue, ClientScope>();
                cfg.CreateMap<ScopeClaim, ScopeClaimValue>();
                cfg.CreateMap<ScopeClaimValue, ScopeClaim>();
                cfg.CreateMap<ScopeSecret, ScopeSecretValue>();
                cfg.CreateMap<ScopeSecretValue, ScopeSecret>();
                cfg.CreateMap<TScope, Scope>();
                cfg.CreateMap<Scope, TScope>();
                cfg.CreateMap<DateTime?, DateTimeOffset?>().ConvertUsing<NullableDateTimeOffsetConverter>();
                cfg.CreateMap<DateTimeOffset?, DateTime?>().ConvertUsing<NullableOffsetDateTimeConverter>();
            });
            _clientMapper = clientConfig.CreateMapper();
        }

        public Task<IdentityAdminMetadata> GetMetadataAsync()
        {
            var updateClient = new List<PropertyMetadata>();
            updateClient.AddRange(PropertyMetadata.FromType<TCLient>());

            var createClient = new List<PropertyMetadata>
            {
                PropertyMetadata.FromProperty<TCLient>(x => x.ClientName,"ClientName", required: true),
                PropertyMetadata.FromProperty<TCLient>(x => x.ClientId,"ClientId", required: true),
            };

            var client = new ClientMetaData
            {
                SupportsCreate = true,
                SupportsDelete = true,
                CreateProperties = createClient,
                UpdateProperties = updateClient
            };


            var updateScope = new List<PropertyMetadata>();
            updateScope.AddRange(PropertyMetadata.FromType<TScope>());

            var createScope = new List<PropertyMetadata>
            {
                PropertyMetadata.FromProperty<TScope>(x => x.Name,"ScopeName", required: true),
            };

            var scope = new ScopeMetaData
            {
                SupportsCreate = true,
                SupportsDelete = true,
                CreateProperties = createScope,
                UpdateProperties = updateScope
            };


            var identityAdminMetadata = new IdentityAdminMetadata
            {
                ClientMetaData = client,
                ScopeMetaData = scope
            };

            return Task.FromResult(identityAdminMetadata);
        }

        #region Scope

        public async Task<IdentityAdminResult<ScopeDetail>> GetScopeAsync(string subject)
        {
                int parsedId;
                if (int.TryParse(subject, out parsedId))
                {
                    var efScope = await _scopeContext.Scopes.FirstOrDefaultAsync(p => p.Id == parsedId);
                    if (efScope == null)
                    {
                        return new IdentityAdminResult<ScopeDetail>((ScopeDetail)null);
                    }
                    var coreScope = new TScope();
                    _clientMapper.Map(efScope, coreScope);
                    var result = new ScopeDetail
                    {
                        Subject = subject,
                        Name = efScope.Name,
                        Description = efScope.Description,
                    };

                    var metadata = await GetMetadataAsync();
                    var props = from prop in metadata.ScopeMetaData.UpdateProperties
                                select new PropertyValue
                                {
                                    Type = prop.Type,
                                    Value = GetScopeProperty(prop, coreScope),
                                };

                    result.Properties = props.ToArray();
                    result.ScopeClaimValues = new List<ScopeClaimValue>();
                    result.ScopeSecretValues = new List<ScopeSecretValue>();
                    _clientMapper.Map(efScope.ScopeClaims.ToList(), result.ScopeClaimValues);
                    _clientMapper.Map(efScope.ScopeSecrets.ToList(), result.ScopeSecretValues);

                    return new IdentityAdminResult<ScopeDetail>(result);
                }
                return new IdentityAdminResult<ScopeDetail>((ScopeDetail)null);
        }

        public Task<IdentityAdminResult<QueryResult<ScopeSummary>>> QueryScopesAsync(string filter, int start, int count)
        {
            {
                var query =
                    from scope in _scopeContext.Scopes
                    orderby scope.Name
                    select scope;

                if (!String.IsNullOrWhiteSpace(filter))
                {
                    query =
                        from scope in query
                        where scope.Name.Contains(filter)
                        orderby scope.Name
                        select scope;
                }

                int total = query.Count();
                var scopes = query.Skip(start).Take(count).ToArray();

                var result = new QueryResult<ScopeSummary>
                {
                    Start = start,
                    Count = count,
                    Total = total,
                    Filter = filter,
                    Items = scopes.Select(x =>
                    {
                        var scope = new ScopeSummary
                        {
                            Subject = x.Id.ToString(),
                            Name = x.Name,
                            Description = x.Name
                        };

                        return scope;
                    }).ToArray()
                };

                return Task.FromResult(new IdentityAdminResult<QueryResult<ScopeSummary>>(result));
            }
        }

        public async Task<IdentityAdminResult<CreateResult>> CreateScopeAsync(IEnumerable<PropertyValue> properties)
        {
            var scopeName = properties.Single(x => x.Type == "ScopeName");
            var scopeNameValue = scopeName.Value;
            string[] exclude = { "ScopeName" };
            var otherProperties = properties.Where(x => !exclude.Contains(x.Type)).ToArray();
            var metadata = await GetMetadataAsync();
            var createProps = metadata.ScopeMetaData.CreateProperties;
            var scope = new TScope { Name = scopeNameValue };

            foreach (var prop in otherProperties)
            {
                var propertyResult = SetScopeProperty(createProps, scope, prop.Type, prop.Value);
                if (!propertyResult.IsSuccess)
                {
                    return new IdentityAdminResult<CreateResult>(propertyResult.Errors.ToArray());
                }
            }
            var efSCope = new Scope();
            {
                try
                {
                    _clientMapper.Map(scope, efSCope);
                    _scopeContext.Scopes.Add(efSCope);
                    _scopeContext.SaveChanges();
                }
                catch (SqlException ex)
                {
                    return new IdentityAdminResult<CreateResult>(ex.Message);
                }
            }

            return new IdentityAdminResult<CreateResult>(new CreateResult { Subject = efSCope.Id.ToString() });
        }

        public async Task<IdentityAdminResult> SetScopePropertyAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                {
                    try
                    {
                        var efScope = await _scopeContext.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (efScope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var meta = await GetMetadataAsync();
                        var coreScope = new TScope();
                        _clientMapper.Map(efScope, coreScope);
                        var propResult = SetScopeProperty(meta.ScopeMetaData.UpdateProperties, coreScope, type, value);
                        if (!propResult.IsSuccess)
                        {
                            return propResult;
                        }
                        _clientMapper.Map(coreScope, efScope);

                        await _scopeContext.SaveChangesAsync();

                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");

        }

        public async Task<IdentityAdminResult> DeleteScopeAsync(string subject)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                {
                    try
                    {
                        var scope = await _scopeContext.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (scope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        _scopeContext.Scopes.Remove(scope);
                        _scopeContext.SaveChanges();
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult(ex.Message);
                    }
                }
            }

            return new IdentityAdminResult("Invalid subject");
        }


        #region Scope claim

        public async Task<IdentityAdminResult> AddScopeClaimAsync(string subject, string name, string description, bool alwaysIncludeInIdToken)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ScopeConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var scope = await _scopeContext.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (scope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClaims = scope.ScopeClaims;
                        if (!existingClaims.Any(x => x.Name == name && x.Description == description))
                        {
                            scope.ScopeClaims.Add(new ScopeClaim
                            {
                                Name = name,
                                Description = description,
                                AlwaysIncludeInIdToken = alwaysIncludeInIdToken
                            });
                            _scopeContext.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveScopeClaimAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedScopeId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedScopeId))
            {
                //using (var db = new ScopeConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var scope = await _scopeContext.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (scope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClaim = scope.ScopeClaims.FirstOrDefault(p => p.Id == parsedScopeId);
                        if (existingClaim != null)
                        {
                            scope.ScopeClaims.Remove(existingClaim);
                            await _scopeContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or clientId");
        }

        public async Task<IdentityAdminResult> UpdateScopeClaim(string subject, string scopeClaimSubject, string name, string description,
            bool alwaysIncludeInIdToken)
        {
            int parsedSubject, parsedScopeClaimSubject;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(scopeClaimSubject, out parsedScopeClaimSubject))
            {
                //using (var db = new ScopeConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var scope = await _scopeContext.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (scope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClaim = scope.ScopeClaims.FirstOrDefault(p => p.Id == parsedScopeClaimSubject);
                        if (existingClaim != null)
                        {
                            existingClaim.AlwaysIncludeInIdToken = alwaysIncludeInIdToken;
                            await _scopeContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or claimId");
        }

        #endregion

        #region scope Secret


        public async Task<IdentityAdminResult> AddScopeSecretAsync(string subject, string type, string value, string description, DateTime? expiration)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ScopeConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var scope = await _scopeContext.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (scope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingSecrets = scope.ScopeSecrets;
                        if (!existingSecrets.Any(x => x.Type == type && x.Value == value))
                        {
                            var scopeSecret = new ScopeSecret
                            {
                                Type = type,
                                Value = value,
                                Description = description

                            };
                            if (expiration.HasValue)
                            {
                                scopeSecret.Expiration = expiration.Value;
                            }
                            scope.ScopeSecrets.Add(scopeSecret);
                            _scopeContext.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> UpdateScopeSecret(string subject, string scopeSecretSubject, string type, string value, string description,
            DateTime? expiration)
        {
            int parsedSubject, parsedScopeSecretSubject;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(scopeSecretSubject, out parsedScopeSecretSubject))
            {
                //using (var db = new ScopeConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var scope = await _scopeContext.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (scope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingSecret = scope.ScopeSecrets.FirstOrDefault(p => p.Id == parsedScopeSecretSubject);
                        if (existingSecret != null)
                        {
                            existingSecret.Value = value;
                            existingSecret.Type = type;
                            existingSecret.Description = description;
                            if (expiration.HasValue)
                            {
                                //Save as new DateTimeOffset(expiration.Value)
                                existingSecret.Expiration = expiration.Value;
                            }
                            await _scopeContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secret id");
        }

        public async Task<IdentityAdminResult> RemoveScopeSecretAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedSecretId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedSecretId))
            {
                //using (var db = new ScopeConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var scope = await _scopeContext.Scopes.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (scope == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingSecret = scope.ScopeSecrets.FirstOrDefault(p => p.Id == parsedSecretId);
                        if (existingSecret != null)
                        {
                            scope.ScopeSecrets.Remove(existingSecret);
                            await _scopeContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }
        #endregion

        #endregion

        #region Client
        public async Task<IdentityAdminResult<ClientDetail>> GetClientAsync(string subject)
        {
            //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
            {
                int parsedId;
                if (int.TryParse(subject, out parsedId))
                {
                    var eFclient = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedId);
                    if (eFclient == null)
                    {
                        return new IdentityAdminResult<ClientDetail>((ClientDetail)null);
                    }
                    var coreClient = new TCLient();
                    _clientMapper.Map(eFclient, coreClient);
                    var result = new ClientDetail
                    {
                        Subject = subject,
                        ClientId = eFclient.ClientId,
                        ClientName = eFclient.ClientName,
                    };


                    var metadata = await GetMetadataAsync();
                    var props = from prop in metadata.ClientMetaData.UpdateProperties
                                select new PropertyValue
                                {
                                    Type = prop.Type,
                                    Value = GetClientProperty(prop, coreClient),
                                };

                    result.Properties = props.ToArray();

                    result.AllowedCorsOrigins = new List<ClientCorsOriginValue>();
                    _clientMapper.Map(eFclient.AllowedCorsOrigins.ToList(), result.AllowedCorsOrigins);
                    result.AllowedCustomGrantTypes = new List<ClientCustomGrantTypeValue>();
                    _clientMapper.Map(eFclient.AllowedCustomGrantTypes.ToList(), result.AllowedCustomGrantTypes);
                    result.AllowedScopes = new List<ClientScopeValue>();
                    _clientMapper.Map(eFclient.AllowedScopes.ToList(), result.AllowedScopes);
                    result.Claims = new List<ClientClaimValue>();
                    _clientMapper.Map(eFclient.Claims.ToList(), result.Claims);
                    result.ClientSecrets = new List<ClientSecretValue>();
                    _clientMapper.Map(eFclient.ClientSecrets.ToList(), result.ClientSecrets);
                    result.IdentityProviderRestrictions = new List<ClientIdPRestrictionValue>();
                    _clientMapper.Map(eFclient.IdentityProviderRestrictions.ToList(), result.IdentityProviderRestrictions);
                    result.PostLogoutRedirectUris = new List<ClientPostLogoutRedirectUriValue>();
                    _clientMapper.Map(eFclient.PostLogoutRedirectUris.ToList(), result.PostLogoutRedirectUris);
                    result.RedirectUris = new List<ClientRedirectUriValue>();
                    _clientMapper.Map(eFclient.RedirectUris.ToList(), result.RedirectUris);

                    return new IdentityAdminResult<ClientDetail>(result);
                }
                return new IdentityAdminResult<ClientDetail>((ClientDetail)null);
            }
        }

        public Task<IdentityAdminResult<QueryResult<ClientSummary>>> QueryClientsAsync(string filter, int start, int count)
        {
            //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
            {
                var query =
                    from client in _clientContext.Clients
                    orderby client.ClientName
                    select client;

                if (!String.IsNullOrWhiteSpace(filter))
                {
                    query =
                        from client in query
                        where client.ClientName.Contains(filter)
                        orderby client.ClientName
                        select client;
                }

                int total = query.Count();
                var clients = query.Skip(start).Take(count).ToArray();

                var result = new QueryResult<ClientSummary>
                {
                    Start = start,
                    Count = count,
                    Total = total,
                    Filter = filter,
                    Items = clients.Select(x =>
                    {
                        var client = new ClientSummary
                        {
                            Subject = x.Id.ToString(),
                            ClientName = x.ClientName,
                            ClientId = x.ClientId
                        };

                        return client;
                    }).ToArray()
                };

                return Task.FromResult(new IdentityAdminResult<QueryResult<ClientSummary>>(result));
            }
        }

        public async Task<IdentityAdminResult<CreateResult>> CreateClientAsync(IEnumerable<PropertyValue> properties)
        {
            var clientNameClaim = properties.Single(x => x.Type == "ClientName");
            var clientIdClaim = properties.Single(x => x.Type == "ClientId");

            var clientId = clientIdClaim.Value;
            var clientName = clientNameClaim.Value;

            string[] exclude = new string[] { "ClientName", "ClientId" };
            var otherProperties = properties.Where(x => !exclude.Contains(x.Type)).ToArray();

            var metadata = await GetMetadataAsync();
            var createProps = metadata.ClientMetaData.CreateProperties;

            var client = new TCLient { ClientId = clientId, ClientName = clientName };
            foreach (var prop in otherProperties)
            {
                var propertyResult = SetClientProperty(createProps, client, prop.Type, prop.Value);
                if (!propertyResult.IsSuccess)
                {
                    return new IdentityAdminResult<CreateResult>(propertyResult.Errors.ToArray());
                }
            }

            var efClient = new Client();
            //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
            {
                try
                {
                    _clientMapper.Map(client, efClient);
                    efClient.Enabled = true;
                    efClient.EnableLocalLogin = true;
                    efClient.RequireConsent = true;
                    efClient.Flow = Flows.Implicit;
                    efClient.AllowClientCredentialsOnly = false;
                    efClient.IdentityTokenLifetime = 300;
                    efClient.AccessTokenLifetime = 3600;
                    efClient.AuthorizationCodeLifetime = 300;
                    efClient.AbsoluteRefreshTokenLifetime = 300;
                    efClient.SlidingRefreshTokenLifetime = 1296000;
                    efClient.AccessTokenType = AccessTokenType.Jwt;
                    efClient.AlwaysSendClientClaims = false;
                    efClient.PrefixClientClaims = true;
                    _clientContext.Clients.Add(efClient);
                    _clientContext.SaveChanges();
                }
                catch (SqlException ex)
                {
                    return new IdentityAdminResult<CreateResult>(ex.Message);
                }
            }

            return new IdentityAdminResult<CreateResult>(new CreateResult { Subject = efClient.Id.ToString() });
        }

        public async Task<IdentityAdminResult> SetClientPropertyAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var eFclient = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (eFclient == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var meta = await GetMetadataAsync();
                        var coreClient = new TCLient();
                        _clientMapper.Map(eFclient, coreClient);
                        var propResult = SetClientProperty(meta.ClientMetaData.UpdateProperties, coreClient, type, value);
                        if (!propResult.IsSuccess)
                        {
                            return propResult;
                        }
                        _clientMapper.Map(coreClient, eFclient);

                        await _clientContext.SaveChangesAsync();

                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");

        }

        public async Task<IdentityAdminResult> DeleteClientAsync(string subject)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        _clientContext.Clients.Remove(client);
                        _clientContext.SaveChanges();
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult(ex.Message);
                    }
                }
            }

            return new IdentityAdminResult("Invalid subject");
        }

        #region Client claim

        public async Task<IdentityAdminResult> AddClientClaimAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClaims = client.Claims;
                        if (!existingClaims.Any(x => x.Type == type && x.Value == value))
                        {
                            client.Claims.Add(new ClientClaim
                            {
                                Type = type,
                                Value = value
                            });
                            _clientContext.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientClaimAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedClientId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedClientId))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClaim = client.Claims.FirstOrDefault(p => p.Id == parsedClientId);
                        if (existingClaim != null)
                        {
                            client.Claims.Remove(existingClaim);
                            await _clientContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or clientId");
        }

        #endregion

        #region Client Secret

        public async Task<IdentityAdminResult> AddClientSecretAsync(string subject, string type, string value)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingSecrets = client.ClientSecrets;
                        if (!existingSecrets.Any(x => x.Type == type && x.Value == value))
                        {
                            client.ClientSecrets.Add(new ClientSecret
                            {
                                Type = type,
                                Value = value
                            });
                            _clientContext.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientSecretAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingClientSecret = client.ClientSecrets.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingClientSecret != null)
                        {
                            client.ClientSecrets.Remove(existingClientSecret);
                            await _clientContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region ClientIdPRestriction

        public async Task<IdentityAdminResult> AddClientIdPRestrictionAsync(string subject, string provider)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingIdentityProviderRestrictions = client.IdentityProviderRestrictions;
                        if (existingIdentityProviderRestrictions.All(x => x.Provider != provider))
                        {
                            client.IdentityProviderRestrictions.Add(new ClientIdPRestriction
                            {
                                Provider = provider,
                            });
                            _clientContext.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientIdPRestrictionAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingIdentityProviderRestrictions = client.IdentityProviderRestrictions.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingIdentityProviderRestrictions != null)
                        {
                            client.IdentityProviderRestrictions.Remove(existingIdentityProviderRestrictions);
                            await _clientContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region PostLogoutRedirectUri

        public async Task<IdentityAdminResult> AddPostLogoutRedirectUriAsync(string subject, string uri)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingPostLogoutRedirectUris = client.PostLogoutRedirectUris;
                        if (existingPostLogoutRedirectUris.All(x => x.Uri != uri))
                        {
                            client.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri
                            {
                                Uri = uri,
                            });
                            _clientContext.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemovePostLogoutRedirectUriAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }

                        var existingPostLogoutRedirectUris = client.PostLogoutRedirectUris.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingPostLogoutRedirectUris != null)
                        {
                            client.PostLogoutRedirectUris.Remove(existingPostLogoutRedirectUris);
                            await _clientContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region ClientRedirectUri

        public async Task<IdentityAdminResult> AddClientRedirectUriAsync(string subject, string uri)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingRedirectUris = client.RedirectUris;
                        if (existingRedirectUris.All(x => x.Uri != uri))
                        {
                            client.RedirectUris.Add(new ClientRedirectUri
                            {
                                Uri = uri,
                            });
                            _clientContext.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientRedirectUriAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingRedirectUris = client.RedirectUris.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingRedirectUris != null)
                        {
                            client.RedirectUris.Remove(existingRedirectUris);
                            await _clientContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region ClientCorsOrigin

        public async Task<IdentityAdminResult> AddClientCorsOriginAsync(string subject, string origin)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingCorsOrigins = client.AllowedCorsOrigins;
                        if (existingCorsOrigins.All(x => x.Origin != origin))
                        {
                            client.AllowedCorsOrigins.Add(new ClientCorsOrigin
                            {
                                Origin = origin,
                            });
                            _clientContext.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientCorsOriginAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingCorsOrigins = client.AllowedCorsOrigins.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingCorsOrigins != null)
                        {
                            client.AllowedCorsOrigins.Remove(existingCorsOrigins);
                            await _clientContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region ClientCustomGrantType

        public async Task<IdentityAdminResult> AddClientCustomGrantTypeAsync(string subject, string grantType)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingGrantTypes = client.AllowedCustomGrantTypes;
                        if (existingGrantTypes.All(x => x.GrantType != grantType))
                        {
                            client.AllowedCustomGrantTypes.Add(new ClientCustomGrantType
                            {
                                GrantType = grantType,
                            });
                            _clientContext.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientCustomGrantTypeAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingGrantTypes = client.AllowedCustomGrantTypes.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingGrantTypes != null)
                        {
                            client.AllowedCustomGrantTypes.Remove(existingGrantTypes);
                            await _clientContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #region ClientScope

        public async Task<IdentityAdminResult> AddClientScopeAsync(string subject, string scope)
        {
            int parsedSubject;
            if (int.TryParse(subject, out parsedSubject))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingScopes = client.AllowedScopes;
                        if (existingScopes.All(x => x.Scope != scope))
                        {
                            client.AllowedScopes.Add(new ClientScope
                            {
                                Scope = scope,
                            });
                            _clientContext.SaveChanges();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject");
        }

        public async Task<IdentityAdminResult> RemoveClientScopeAsync(string subject, string id)
        {
            int parsedSubject;
            int parsedObjectId;
            if (int.TryParse(subject, out parsedSubject) && int.TryParse(id, out parsedObjectId))
            {
                //using (var db = new ClientConfigurationDbContext(_entityFrameworkServiceOptions.ConnectionString, _entityFrameworkServiceOptions.Schema))
                {
                    try
                    {
                        var client = await _clientContext.Clients.FirstOrDefaultAsync(p => p.Id == parsedSubject);
                        if (client == null)
                        {
                            return new IdentityAdminResult("Invalid subject");
                        }
                        var existingScopes = client.AllowedScopes.FirstOrDefault(p => p.Id == parsedObjectId);
                        if (existingScopes != null)
                        {
                            client.AllowedScopes.Remove(existingScopes);
                            await _clientContext.SaveChangesAsync();
                        }
                        return IdentityAdminResult.Success;
                    }
                    catch (SqlException ex)
                    {
                        return new IdentityAdminResult<CreateResult>(ex.Message);
                    }
                }
            }
            return new IdentityAdminResult("Invalid subject or secretId");
        }

        #endregion

        #endregion

        #region helperMethods
        protected IdentityAdminResult SetClientProperty(IEnumerable<PropertyMetadata> propsMeta, TCLient client, string type, string value)
        {
            IdentityAdminResult result;
            if (propsMeta.TrySet(client, type, value, out result))
            {
                return result;
            }

            throw new Exception("Invalid property type " + type);
        }

        protected string GetClientProperty(PropertyMetadata propMetadata, TCLient client)
        {
            string val;
            if (propMetadata.TryGet(client, out val))
            {
                return val;
            }
            throw new Exception("Invalid property type " + propMetadata.Type);
        }

        protected IdentityAdminResult SetScopeProperty(IEnumerable<PropertyMetadata> propsMeta, TScope scope, string type, string value)
        {
            IdentityAdminResult result;
            if (propsMeta.TrySet(scope, type, value, out result))
            {
                return result;
            }

            throw new Exception("Invalid property type " + type);
        }

        protected string GetScopeProperty(PropertyMetadata propMetadata, TScope scope)
        {
            string val;
            if (propMetadata.TryGet(scope, out val))
            {
                return val;
            }
            throw new Exception("Invalid property type " + propMetadata.Type);
        }


        #endregion
    }

    public class NullableOffsetDateTimeConverter : TypeConverter<System.DateTimeOffset?, System.DateTime?>
    {
        /// <summary>
        /// Converts data from DateTime to DateTimeOffset
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected override System.DateTime? ConvertCore(System.DateTimeOffset? source)
        {
            if (source.HasValue)
                if (source.Value.Offset.Equals(TimeSpan.Zero))
                    return source.Value.UtcDateTime;
                else if (source.Value.Offset.Equals(TimeZoneInfo.Local.GetUtcOffset(source.Value.DateTime)))
                    return DateTime.SpecifyKind(source.Value.DateTime, DateTimeKind.Local);
                else
                    return source.Value.DateTime;
            else
                return null;
        }
    }

    public class NullableDateTimeOffsetConverter : TypeConverter<System.DateTime?, System.DateTimeOffset?>
    {
        /// <summary>
        /// Converts data from DateTime to DateTimeOffset
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected override System.DateTimeOffset? ConvertCore(System.DateTime? source)
        {
            if (source.HasValue)
                return source.Value;
            else
                return null;
        }
    }


}
