﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VistasSorrySliders.ServicioSorrySliders {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Constantes", Namespace="http://schemas.datacontract.org/2004/07/DatosSorrySliders")]
    public enum Constantes : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ERROR_CONEXION_BD = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ERROR_CONSULTA = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        ERROR_CONEXION_SERVIDOR = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OPERACION_EXITOSA = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        OPERACION_EXITOSA_VACIA = 4,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CuentaSet", Namespace="http://schemas.datacontract.org/2004/07/DatosSorrySliders")]
    [System.SerializableAttribute()]
    public partial class CuentaSet : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte[] AvatarField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ContraseñaField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CorreoElectronicoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdUsuarioField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NicknameField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte[] Avatar {
            get {
                return this.AvatarField;
            }
            set {
                if ((object.ReferenceEquals(this.AvatarField, value) != true)) {
                    this.AvatarField = value;
                    this.RaisePropertyChanged("Avatar");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Contraseña {
            get {
                return this.ContraseñaField;
            }
            set {
                if ((object.ReferenceEquals(this.ContraseñaField, value) != true)) {
                    this.ContraseñaField = value;
                    this.RaisePropertyChanged("Contraseña");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CorreoElectronico {
            get {
                return this.CorreoElectronicoField;
            }
            set {
                if ((object.ReferenceEquals(this.CorreoElectronicoField, value) != true)) {
                    this.CorreoElectronicoField = value;
                    this.RaisePropertyChanged("CorreoElectronico");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int IdUsuario {
            get {
                return this.IdUsuarioField;
            }
            set {
                if ((this.IdUsuarioField.Equals(value) != true)) {
                    this.IdUsuarioField = value;
                    this.RaisePropertyChanged("IdUsuario");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Nickname {
            get {
                return this.NicknameField;
            }
            set {
                if ((object.ReferenceEquals(this.NicknameField, value) != true)) {
                    this.NicknameField = value;
                    this.RaisePropertyChanged("Nickname");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UsuarioSet", Namespace="http://schemas.datacontract.org/2004/07/DatosSorrySliders")]
    [System.SerializableAttribute()]
    public partial class UsuarioSet : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ApellidoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private VistasSorrySliders.ServicioSorrySliders.CuentaSet[] CuentaSetField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IdUsuarioField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NombreField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Apellido {
            get {
                return this.ApellidoField;
            }
            set {
                if ((object.ReferenceEquals(this.ApellidoField, value) != true)) {
                    this.ApellidoField = value;
                    this.RaisePropertyChanged("Apellido");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public VistasSorrySliders.ServicioSorrySliders.CuentaSet[] CuentaSet {
            get {
                return this.CuentaSetField;
            }
            set {
                if ((object.ReferenceEquals(this.CuentaSetField, value) != true)) {
                    this.CuentaSetField = value;
                    this.RaisePropertyChanged("CuentaSet");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int IdUsuario {
            get {
                return this.IdUsuarioField;
            }
            set {
                if ((this.IdUsuarioField.Equals(value) != true)) {
                    this.IdUsuarioField = value;
                    this.RaisePropertyChanged("IdUsuario");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Nombre {
            get {
                return this.NombreField;
            }
            set {
                if ((object.ReferenceEquals(this.NombreField, value) != true)) {
                    this.NombreField = value;
                    this.RaisePropertyChanged("Nombre");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PartidaSet", Namespace="http://schemas.datacontract.org/2004/07/DatosSorrySliders")]
    [System.SerializableAttribute()]
    public partial class PartidaSet : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CantidadJugadoresField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid CodigoPartidaField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CorreoElectronicoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private VistasSorrySliders.ServicioSorrySliders.CuentaSet CuentaSetField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CantidadJugadores {
            get {
                return this.CantidadJugadoresField;
            }
            set {
                if ((this.CantidadJugadoresField.Equals(value) != true)) {
                    this.CantidadJugadoresField = value;
                    this.RaisePropertyChanged("CantidadJugadores");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid CodigoPartida {
            get {
                return this.CodigoPartidaField;
            }
            set {
                if ((this.CodigoPartidaField.Equals(value) != true)) {
                    this.CodigoPartidaField = value;
                    this.RaisePropertyChanged("CodigoPartida");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CorreoElectronico {
            get {
                return this.CorreoElectronicoField;
            }
            set {
                if ((object.ReferenceEquals(this.CorreoElectronicoField, value) != true)) {
                    this.CorreoElectronicoField = value;
                    this.RaisePropertyChanged("CorreoElectronico");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public VistasSorrySliders.ServicioSorrySliders.CuentaSet CuentaSet {
            get {
                return this.CuentaSetField;
            }
            set {
                if ((object.ReferenceEquals(this.CuentaSetField, value) != true)) {
                    this.CuentaSetField = value;
                    this.RaisePropertyChanged("CuentaSet");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicioSorrySliders.IInicioSesion")]
    public interface IInicioSesion {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInicioSesion/VerificarExistenciaCorreoCuenta", ReplyAction="http://tempuri.org/IInicioSesion/VerificarExistenciaCorreoCuentaResponse")]
        VistasSorrySliders.ServicioSorrySliders.Constantes VerificarExistenciaCorreoCuenta(string correoElectronico);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInicioSesion/VerificarExistenciaCorreoCuenta", ReplyAction="http://tempuri.org/IInicioSesion/VerificarExistenciaCorreoCuentaResponse")]
        System.Threading.Tasks.Task<VistasSorrySliders.ServicioSorrySliders.Constantes> VerificarExistenciaCorreoCuentaAsync(string correoElectronico);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInicioSesion/VerificarContrasenaDeCuenta", ReplyAction="http://tempuri.org/IInicioSesion/VerificarContrasenaDeCuentaResponse")]
        VistasSorrySliders.ServicioSorrySliders.Constantes VerificarContrasenaDeCuenta(VistasSorrySliders.ServicioSorrySliders.CuentaSet cuentaPorVerificar);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInicioSesion/VerificarContrasenaDeCuenta", ReplyAction="http://tempuri.org/IInicioSesion/VerificarContrasenaDeCuentaResponse")]
        System.Threading.Tasks.Task<VistasSorrySliders.ServicioSorrySliders.Constantes> VerificarContrasenaDeCuentaAsync(VistasSorrySliders.ServicioSorrySliders.CuentaSet cuentaPorVerificar);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IInicioSesionChannel : VistasSorrySliders.ServicioSorrySliders.IInicioSesion, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class InicioSesionClient : System.ServiceModel.ClientBase<VistasSorrySliders.ServicioSorrySliders.IInicioSesion>, VistasSorrySliders.ServicioSorrySliders.IInicioSesion {
        
        public InicioSesionClient() {
        }
        
        public InicioSesionClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public InicioSesionClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public InicioSesionClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public InicioSesionClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VistasSorrySliders.ServicioSorrySliders.Constantes VerificarExistenciaCorreoCuenta(string correoElectronico) {
            return base.Channel.VerificarExistenciaCorreoCuenta(correoElectronico);
        }
        
        public System.Threading.Tasks.Task<VistasSorrySliders.ServicioSorrySliders.Constantes> VerificarExistenciaCorreoCuentaAsync(string correoElectronico) {
            return base.Channel.VerificarExistenciaCorreoCuentaAsync(correoElectronico);
        }
        
        public VistasSorrySliders.ServicioSorrySliders.Constantes VerificarContrasenaDeCuenta(VistasSorrySliders.ServicioSorrySliders.CuentaSet cuentaPorVerificar) {
            return base.Channel.VerificarContrasenaDeCuenta(cuentaPorVerificar);
        }
        
        public System.Threading.Tasks.Task<VistasSorrySliders.ServicioSorrySliders.Constantes> VerificarContrasenaDeCuentaAsync(VistasSorrySliders.ServicioSorrySliders.CuentaSet cuentaPorVerificar) {
            return base.Channel.VerificarContrasenaDeCuentaAsync(cuentaPorVerificar);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicioSorrySliders.IMenuPrincipal")]
    public interface IMenuPrincipal {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMenuPrincipal/RecuperarDatosUsuario", ReplyAction="http://tempuri.org/IMenuPrincipal/RecuperarDatosUsuarioResponse")]
        System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, string, byte[]> RecuperarDatosUsuario(string correoElectronico);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IMenuPrincipal/RecuperarDatosUsuario", ReplyAction="http://tempuri.org/IMenuPrincipal/RecuperarDatosUsuarioResponse")]
        System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, string, byte[]>> RecuperarDatosUsuarioAsync(string correoElectronico);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IMenuPrincipalChannel : VistasSorrySliders.ServicioSorrySliders.IMenuPrincipal, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class MenuPrincipalClient : System.ServiceModel.ClientBase<VistasSorrySliders.ServicioSorrySliders.IMenuPrincipal>, VistasSorrySliders.ServicioSorrySliders.IMenuPrincipal {
        
        public MenuPrincipalClient() {
        }
        
        public MenuPrincipalClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public MenuPrincipalClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MenuPrincipalClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public MenuPrincipalClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, string, byte[]> RecuperarDatosUsuario(string correoElectronico) {
            return base.Channel.RecuperarDatosUsuario(correoElectronico);
        }
        
        public System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, string, byte[]>> RecuperarDatosUsuarioAsync(string correoElectronico) {
            return base.Channel.RecuperarDatosUsuarioAsync(correoElectronico);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicioSorrySliders.IRegistroUsuario")]
    public interface IRegistroUsuario {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRegistroUsuario/AgregarUsuario", ReplyAction="http://tempuri.org/IRegistroUsuario/AgregarUsuarioResponse")]
        VistasSorrySliders.ServicioSorrySliders.Constantes AgregarUsuario(VistasSorrySliders.ServicioSorrySliders.UsuarioSet usuarioPorGuardar, VistasSorrySliders.ServicioSorrySliders.CuentaSet cuentaPorGuardar);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRegistroUsuario/AgregarUsuario", ReplyAction="http://tempuri.org/IRegistroUsuario/AgregarUsuarioResponse")]
        System.Threading.Tasks.Task<VistasSorrySliders.ServicioSorrySliders.Constantes> AgregarUsuarioAsync(VistasSorrySliders.ServicioSorrySliders.UsuarioSet usuarioPorGuardar, VistasSorrySliders.ServicioSorrySliders.CuentaSet cuentaPorGuardar);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IRegistroUsuarioChannel : VistasSorrySliders.ServicioSorrySliders.IRegistroUsuario, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class RegistroUsuarioClient : System.ServiceModel.ClientBase<VistasSorrySliders.ServicioSorrySliders.IRegistroUsuario>, VistasSorrySliders.ServicioSorrySliders.IRegistroUsuario {
        
        public RegistroUsuarioClient() {
        }
        
        public RegistroUsuarioClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public RegistroUsuarioClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RegistroUsuarioClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public RegistroUsuarioClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VistasSorrySliders.ServicioSorrySliders.Constantes AgregarUsuario(VistasSorrySliders.ServicioSorrySliders.UsuarioSet usuarioPorGuardar, VistasSorrySliders.ServicioSorrySliders.CuentaSet cuentaPorGuardar) {
            return base.Channel.AgregarUsuario(usuarioPorGuardar, cuentaPorGuardar);
        }
        
        public System.Threading.Tasks.Task<VistasSorrySliders.ServicioSorrySliders.Constantes> AgregarUsuarioAsync(VistasSorrySliders.ServicioSorrySliders.UsuarioSet usuarioPorGuardar, VistasSorrySliders.ServicioSorrySliders.CuentaSet cuentaPorGuardar) {
            return base.Channel.AgregarUsuarioAsync(usuarioPorGuardar, cuentaPorGuardar);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicioSorrySliders.ILobby", CallbackContract=typeof(VistasSorrySliders.ServicioSorrySliders.ILobbyCallback))]
    public interface ILobby {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ILobby/EntrarPartida")]
        void EntrarPartida(string uid, string correoJugadorNuevo);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ILobby/EntrarPartida")]
        System.Threading.Tasks.Task EntrarPartidaAsync(string uid, string correoJugadorNuevo);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILobbyCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ILobby/JugadorEntroPartida")]
        void JugadorEntroPartida(VistasSorrySliders.ServicioSorrySliders.CuentaSet[] listaJugadores);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILobbyChannel : VistasSorrySliders.ServicioSorrySliders.ILobby, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LobbyClient : System.ServiceModel.DuplexClientBase<VistasSorrySliders.ServicioSorrySliders.ILobby>, VistasSorrySliders.ServicioSorrySliders.ILobby {
        
        public LobbyClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public LobbyClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public LobbyClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public LobbyClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public LobbyClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void EntrarPartida(string uid, string correoJugadorNuevo) {
            base.Channel.EntrarPartida(uid, correoJugadorNuevo);
        }
        
        public System.Threading.Tasks.Task EntrarPartidaAsync(string uid, string correoJugadorNuevo) {
            return base.Channel.EntrarPartidaAsync(uid, correoJugadorNuevo);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicioSorrySliders.IUnirsePartida")]
    public interface IUnirsePartida {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUnirsePartida/UnirseAlLobby", ReplyAction="http://tempuri.org/IUnirsePartida/UnirseAlLobbyResponse")]
        System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, int> UnirseAlLobby(string uid, string correoJugadorNuevo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUnirsePartida/UnirseAlLobby", ReplyAction="http://tempuri.org/IUnirsePartida/UnirseAlLobbyResponse")]
        System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, int>> UnirseAlLobbyAsync(string uid, string correoJugadorNuevo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUnirsePartida/RecuperarJugadoresLobby", ReplyAction="http://tempuri.org/IUnirsePartida/RecuperarJugadoresLobbyResponse")]
        System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, VistasSorrySliders.ServicioSorrySliders.CuentaSet[]> RecuperarJugadoresLobby(string uid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUnirsePartida/RecuperarJugadoresLobby", ReplyAction="http://tempuri.org/IUnirsePartida/RecuperarJugadoresLobbyResponse")]
        System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, VistasSorrySliders.ServicioSorrySliders.CuentaSet[]>> RecuperarJugadoresLobbyAsync(string uid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUnirsePartida/RecuperarPartida", ReplyAction="http://tempuri.org/IUnirsePartida/RecuperarPartidaResponse")]
        System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, VistasSorrySliders.ServicioSorrySliders.PartidaSet> RecuperarPartida(string codigoPartida);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IUnirsePartida/RecuperarPartida", ReplyAction="http://tempuri.org/IUnirsePartida/RecuperarPartidaResponse")]
        System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, VistasSorrySliders.ServicioSorrySliders.PartidaSet>> RecuperarPartidaAsync(string codigoPartida);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IUnirsePartidaChannel : VistasSorrySliders.ServicioSorrySliders.IUnirsePartida, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UnirsePartidaClient : System.ServiceModel.ClientBase<VistasSorrySliders.ServicioSorrySliders.IUnirsePartida>, VistasSorrySliders.ServicioSorrySliders.IUnirsePartida {
        
        public UnirsePartidaClient() {
        }
        
        public UnirsePartidaClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public UnirsePartidaClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UnirsePartidaClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UnirsePartidaClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, int> UnirseAlLobby(string uid, string correoJugadorNuevo) {
            return base.Channel.UnirseAlLobby(uid, correoJugadorNuevo);
        }
        
        public System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, int>> UnirseAlLobbyAsync(string uid, string correoJugadorNuevo) {
            return base.Channel.UnirseAlLobbyAsync(uid, correoJugadorNuevo);
        }
        
        public System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, VistasSorrySliders.ServicioSorrySliders.CuentaSet[]> RecuperarJugadoresLobby(string uid) {
            return base.Channel.RecuperarJugadoresLobby(uid);
        }
        
        public System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, VistasSorrySliders.ServicioSorrySliders.CuentaSet[]>> RecuperarJugadoresLobbyAsync(string uid) {
            return base.Channel.RecuperarJugadoresLobbyAsync(uid);
        }
        
        public System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, VistasSorrySliders.ServicioSorrySliders.PartidaSet> RecuperarPartida(string codigoPartida) {
            return base.Channel.RecuperarPartida(codigoPartida);
        }
        
        public System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, VistasSorrySliders.ServicioSorrySliders.PartidaSet>> RecuperarPartidaAsync(string codigoPartida) {
            return base.Channel.RecuperarPartidaAsync(codigoPartida);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicioSorrySliders.ICrearLobby")]
    public interface ICrearLobby {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICrearLobby/CrearPartida", ReplyAction="http://tempuri.org/ICrearLobby/CrearPartidaResponse")]
        System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, string> CrearPartida(string correoHost, int NumeroJugadores);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICrearLobby/CrearPartida", ReplyAction="http://tempuri.org/ICrearLobby/CrearPartidaResponse")]
        System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, string>> CrearPartidaAsync(string correoHost, int NumeroJugadores);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICrearLobbyChannel : VistasSorrySliders.ServicioSorrySliders.ICrearLobby, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CrearLobbyClient : System.ServiceModel.ClientBase<VistasSorrySliders.ServicioSorrySliders.ICrearLobby>, VistasSorrySliders.ServicioSorrySliders.ICrearLobby {
        
        public CrearLobbyClient() {
        }
        
        public CrearLobbyClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CrearLobbyClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CrearLobbyClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CrearLobbyClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, string> CrearPartida(string correoHost, int NumeroJugadores) {
            return base.Channel.CrearPartida(correoHost, NumeroJugadores);
        }
        
        public System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioSorrySliders.Constantes, string>> CrearPartidaAsync(string correoHost, int NumeroJugadores) {
            return base.Channel.CrearPartidaAsync(correoHost, NumeroJugadores);
        }
    }
}
