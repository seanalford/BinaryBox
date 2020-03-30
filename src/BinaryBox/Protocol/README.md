# Binary Box Core

## How to build a custom protocol.

#### Create Protocol Project

1. Create a new project. (i.e. _**[ProtocolOwner]**_.Protocol._**[ProtocolName]**_)

    **NOTE:** Replace _**[ProtocolOwner]**_ with the protocol owner, and _**[ProtocolName]**_ with the target protocol.

2. Add a referance to the BinaryBox NuGet/MyGet package.

3. Create the following project folders.

- Client
- Message
- Settings

#### Create Protocol Settings

In the projects **Settings** folder add the following boiler plate code.
    
  - I[_**ProtocolName**_]ProtocolSettings.cs
    
    ```csharp
    namespace [ProtocolOwner].Protocol.[ProtocolName]
    {
        public interface I[ProtocolName]ProtocolSettings : IProtocolSettings
        {
            // TODO: Add protocol setting properties here.
        }
    }
    ```

  - **ProtocolName**ProtocolSettings.cs

    ```csharp
    namespace [ProtocolOwner].Protocol.[ProtocolName]
    {
        public class [ProtocolName]ProtocolSettings : ProtocolSettings, I[ProtocolName]ProtocolSettings
        {
            // TODO: Add protocol setting properties here.
        }
    }
    ```

#### Create Protocol Messsage

In the projects **Message** folder add the following boiler place code.

  - [_**ProtocolName**_]ProtocolMessageTypes.cs
    ```csharp
    namespace [ProtocolOwner].Protocol.[ProtocolName]
    {
        public enum [ProtocolName]ProtcolMessageTypes 
        {
            // TODO: Add protocol message types (functions) here.
        }
    }
    ```  

  - I[_**ProtocolName**_]ProtocolMessage.cs
    ```csharp
    namespace [ProtocolOwner].Protocol.[ProtocolName]
    {
        public interface I[ProtocolName]ProtocolMessage<TProtocolSettings> : IProtocolMessage<TProtocolSettings>
            where TProtocolSettings : I[ProtocolName]ProtocolSettings
        {

        }
    }
    ```  

  - I[_**ProtocolName**_]ProtocolMessageData.cs
    ```csharp
    namespace [ProtocolOwner].Protocol.[ProtocolName]
    {
        public interface I[ProtocolName]ProtocolMessageData : IProtocolMessageData
        {

        }
    }
    ```  

  - [_**ProtocolName**_]ProtocolMessageData.cs
    ```csharp
    namespace [ProtocolOwner].Protocol.[ProtocolName]
    {
        public class [ProtocolName]ProtocolMessageData : I[ProtocolName]ProtocolMessageData
        {
            public void Clear()
            {
                // TODO: Restore default values.            
            }
        }
    }
    ```

  - [_**ProtocolName**_]ProtocolMessage.cs  
    ```csharp
    using Microsoft.Extensions.Logging;

    namespace [ProtocolOwner].Protocol.[ProtocolName]
    {
        public abstract class [ProtocolName]ProtocolMessage : ProtocolMessage<I[ProtocolName]ProtocolSettings>, I[ProtocolName]ProtocolMessage<I[ProtocolName]ProtocolSettings>
        {
            // TODO: Add protocol properties here.

            public [ProtocolName]ProtocolMessage(ILogger logger, I[ProtocolName]ProtocolSettings settings) : base(logger, settings)
            {
                // TODO: Add init code here.
            }
        }
    }
    ```

#### Protocol Client

In the projects **Client** folder add the following boiler place code.

  - [_**ProtocolName**_]ProtocolClient.cs

    ```csharp
    using BinaryBox.Connection;
    using Microsoft.Extensions.Logging;

    namespace [ProtocolOwner].Protocol.[ProtocolName]
    {
        public class [ProtocolName]ProtocolClient : ProtocolClient<I[ProtocolName]ProtocolSettings, I[ProtocolName]ProtocolMessage<I[ProtocolName]ProtocolSettings>>
        {
            public [ProtocolName]ProtocolClient(ILogger logger, IConnection connection, I[ProtocolName]ProtocolSettings settings) : base(logger, connection, settings)
            {
                // TODO: Add initialization code.
            }
            public override void Dispose()
            {
                // TODO: Add Dispose code.
            }
        }
    }
    ```




