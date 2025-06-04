namespace Visus.AddressValidation.Integration.Google.Tests;

using Microsoft.Extensions.Configuration;

public sealed class ConfigurationFixture
{
    public IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                  .AddInMemoryCollection(new List<KeyValuePair<string, string?>>
                                                   {
                                                       new(Constants.PrivateKeyConfigurationKey, """
                                                                                                 -----BEGIN PRIVATE KEY-----
                                                                                                 MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQCsDXy/dTyng+bp
                                                                                                 jM8sW3fjaA1MQw6Vok2p3I/P21Xjb2f6N6aKNJ8z3TjiHohfLfeYmK2hs4XGa+dv
                                                                                                 ofpG7uTMQ9NDdLcQzH72fHm/rjyGuAAaPzqrocWGzw3wk2O/kEZ7ZkJqz5s4H16G
                                                                                                 pXwaaSMf496dsE17igXblTbbdK+Hqz1vgxiYEIHoFow04WvcUCSaoH9X8m8QnLEW
                                                                                                 wCf+13NvgH7wmmw0/ZDB1JDeYb17C9/4P+fQBMRchHuZwGENECuYWarV1TlKoHeS
                                                                                                 Nx/0iPD6Ji2uuzdg12LjtnPjgSsg9pgc6lNYFRTnZa18JxkoUblM2BWanu4irrbt
                                                                                                 PKKez7L5AgMBAAECggEAEBTEo6qEDE42T38DQCyedIldsNKVsuO0KZu9b1qbWQIp
                                                                                                 MEhyTvmjJbhYID5lVawYt9ERbYs3hjfArnzvxSCADx2JygTHNZE/jq9Mu98/tgHN
                                                                                                 tKZTSAZ7TWHd9i34hUepZtBEhfI4mlLIRYKDKn7IMytbu9ZmL5VTfdYhBfsOivoN
                                                                                                 kEzexyUnwBVj3+Z4z2HOvzPeKLkv+FLaNg9qCFB/+viQG6TIhz3zjvsxG1a3/lf3
                                                                                                 Za1/m9G0RdmjYc8r7YVFM9nXMgbs9n08PvZjAQDYo3Cj+yTlZ5Xr9c1/yGsHU79j
                                                                                                 ZP7Pgp+XQIID9HZVRE1aaSyPuwuj085PTy6s9JISXwKBgQDiWtugAIYOaZ84RZ5o
                                                                                                 CBGVXnfrmdlEVFzNQwli+7qybSvYjWCnbmX1MZSdm35ppCEUnbEqc8yL+fqbSzMc
                                                                                                 ji4sKZRv07qwIamGbLiWMsOIEORPknpRsmC0/Ymh6Q2UbTd9V022tRA5HMfiiF5h
                                                                                                 aIWfkTqm2GGJx7dSGgtRaPcSHwKBgQDClgE3mp2IpuT+P3J2KJj+PEdyWLdSlYPt
                                                                                                 O+W9kEPhW5OmG3jzpR4FTSQ3nJT+lIgMq14k2JoYLngFZl8OWGzoINi0MO1WbW7w
                                                                                                 eDI0m/nf1zZQtuQS8Yw+CDd+oGrdXJmhtDJVUhVkiE0n26MIQb79Wshrk/sdShjt
                                                                                                 cevU2LCH5wKBgQDPxQ5rY4+pkxHvGRg57Y4WSUxSGjnwGm/EiZAIJ4BLXyIr+DmL
                                                                                                 9i3oTsZXlO8IEPu8bLK+gOR0Z9S8zt3vjCKdrtzteK+YFI4DMbCNTbNlJfwrfgyB
                                                                                                 CPbzqvW2hLFOWKHij0xqNPDbO6vOJ9ZaGxLsUOZBV8TQL136IqXY6DxBiQKBgQDB
                                                                                                 9N2fvClcpebJmxqPtqXRfOpGizGoSspmtPaqPlu7DRoeT3H/gk1rQVphaF2HaSw3
                                                                                                 XLWJirIGeoM99q23UeK0etEmw0I/jPMxFM4ObI7kVNOaHsM2Mrj0uCIAwQvkPEIS
                                                                                                 0df9/cS/IbGukhpux4IFbfEqciWzK0GvpIdK8Pb9bwKBgQCk5ZsQAjyK/RFIoPXS
                                                                                                 NNmrILW0XxaUzXSoBgmAtTFrTgnNZ6qCNo4jxsurndFqiXs57Hplop4BsGqBYEUR
                                                                                                 nRiWziqpjEBv5RQpqWBM/ZGPh+UYa/3H5dILQO3cwqdx7luHUcNNu76EvoCWczzy
                                                                                                 LPcr8vAfAJCzZVMEdBEr4kuLHw==
                                                                                                 -----END PRIVATE KEY----
                                                                                                 """),
                                                       new(Constants.ProjectIdConfigurationKey, "d8ytrt8dr16v"),
                                                       new(Constants.ServiceAccountEmailConfigurationKey, "test@d8ytrt8dr16v.iam.gserviceaccount.test")
                                                   })
                                                  .Build(); // remark: these values are fake
}
