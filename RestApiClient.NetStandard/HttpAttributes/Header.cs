using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace TheProcessE.RestApiClient
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public partial class Header : Attribute
    {
        public string Key { get; }
        public string Value { get; }
        private IAuthModel authModel;
        private IAuthModel AuthModel => (authModel != null || authModel != default) ? authModel : _authModelType != null ? authModel = Create(_authModelType) : default;

        internal bool HasAuthModel => AuthModel != null && !string.IsNullOrWhiteSpace(AuthModel.Scheme) && !string.IsNullOrWhiteSpace(AuthModel.Token);

        public AuthenticationHeaderValue Authentication => new AuthenticationHeaderValue(AuthModel.Scheme, AuthModel.Token);

        public Header(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException($"{nameof(key)} or {nameof(value)} are null or empty on Header Attribute!");

            Key = key;
            Value = value;
        }

        public Header(Type AuthModelType)
        {
            if (AuthModelType == null)
                throw new ArgumentNullException(nameof(AuthModelType));

            object Instance = default;

            try
            {
                Instance = Activator.CreateInstance(AuthModelType);
            }
            catch
            {
                throw new ArgumentException($"make sure that {nameof(AuthModelType)} has default constructor");
            }

            if (!AuthModelType.IsAssignableFrom(typeof(IAuthModel)) && AuthModelType.GetInterface(nameof(IAuthModel)) == null)
                throw new ArgumentException($"{nameof(AuthModelType)} does not implement {typeof(IAuthModel)}");
            Key = "Authorization";
            Value = string.Empty;
            _authModelType = AuthModelType;
            authModel = ((IAuthModel)Instance).Create() ;
        }

    }

    public partial class Header : Attribute
    {
        private Type _authModelType { get; }

        internal IAuthModel Create<T>() where T : IAuthModel, new()
        {
            return Activator.CreateInstance<T>().Create();
        }

        private IAuthModel Create(Type type)
        {
            return ((IAuthModel)Activator.CreateInstance(type)).Create();
        }

    }
}
