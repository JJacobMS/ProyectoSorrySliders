﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VistasSorrySliders.ServicioInicioSesion {
    using System.Runtime.Serialization;
    using System;
    
    
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServicioInicioSesion.IInicioSesion")]
    public interface IInicioSesion {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInicioSesion/VerificarExistenciaCorreoCuenta", ReplyAction="http://tempuri.org/IInicioSesion/VerificarExistenciaCorreoCuentaResponse")]
        System.ValueTuple<string, int> VerificarExistenciaCorreoCuenta(string correoElectronico);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInicioSesion/VerificarExistenciaCorreoCuenta", ReplyAction="http://tempuri.org/IInicioSesion/VerificarExistenciaCorreoCuentaResponse")]
        System.Threading.Tasks.Task<System.ValueTuple<string, int>> VerificarExistenciaCorreoCuentaAsync(string correoElectronico);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInicioSesion/VerificarContrasenaDeCuenta", ReplyAction="http://tempuri.org/IInicioSesion/VerificarContrasenaDeCuentaResponse")]
        System.ValueTuple<VistasSorrySliders.ServicioInicioSesion.CuentaSet, int> VerificarContrasenaDeCuenta(VistasSorrySliders.ServicioInicioSesion.CuentaSet cuentaPorVerificar);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IInicioSesion/VerificarContrasenaDeCuenta", ReplyAction="http://tempuri.org/IInicioSesion/VerificarContrasenaDeCuentaResponse")]
        System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioInicioSesion.CuentaSet, int>> VerificarContrasenaDeCuentaAsync(VistasSorrySliders.ServicioInicioSesion.CuentaSet cuentaPorVerificar);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IInicioSesionChannel : VistasSorrySliders.ServicioInicioSesion.IInicioSesion, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class InicioSesionClient : System.ServiceModel.ClientBase<VistasSorrySliders.ServicioInicioSesion.IInicioSesion>, VistasSorrySliders.ServicioInicioSesion.IInicioSesion {
        
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
        
        public System.ValueTuple<string, int> VerificarExistenciaCorreoCuenta(string correoElectronico) {
            return base.Channel.VerificarExistenciaCorreoCuenta(correoElectronico);
        }
        
        public System.Threading.Tasks.Task<System.ValueTuple<string, int>> VerificarExistenciaCorreoCuentaAsync(string correoElectronico) {
            return base.Channel.VerificarExistenciaCorreoCuentaAsync(correoElectronico);
        }
        
        public System.ValueTuple<VistasSorrySliders.ServicioInicioSesion.CuentaSet, int> VerificarContrasenaDeCuenta(VistasSorrySliders.ServicioInicioSesion.CuentaSet cuentaPorVerificar) {
            return base.Channel.VerificarContrasenaDeCuenta(cuentaPorVerificar);
        }
        
        public System.Threading.Tasks.Task<System.ValueTuple<VistasSorrySliders.ServicioInicioSesion.CuentaSet, int>> VerificarContrasenaDeCuentaAsync(VistasSorrySliders.ServicioInicioSesion.CuentaSet cuentaPorVerificar) {
            return base.Channel.VerificarContrasenaDeCuentaAsync(cuentaPorVerificar);
        }
    }
}
