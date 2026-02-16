# Build and Deployment Guide

This guide covers building and deploying the Body Measurement App for all supported platforms.

## Prerequisites

### Common Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio 2022 (17.9+)](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Platform-Specific Requirements

#### iOS
- macOS 13+ (Ventura or later)
- Xcode 15+
- Apple Developer Account (for device deployment)
- iOS SDK 17+

#### Android
- Android SDK 34+
- Java JDK 17+
- Android Emulator or physical device

#### Windows
- Windows 11 SDK (10.0.19041.0 or later)
- Windows 10/11 (for running the app)

#### macOS
- macOS 13+
- Xcode 15+

## Development Builds

### Command Line

#### Android

```bash
# Debug build
dotnet build -t:Run -f net10.0-android

# Specific device
dotnet build -t:Run -f net10.0-android -p:AndroidDevice="device-id"

# List available devices
adb devices
```

#### iOS

```bash
# Simulator (Debug)
dotnet build -t:Run -f net10.0-ios -p:_DeviceName=:v2:udid=<simulator-udid>

# List available simulators
xcrun simctl list devices

# Physical device (requires provisioning profile)
dotnet build -t:Run -f net10.0-ios -p:_DeviceName=<device-name>
```

#### Windows

```bash
# Debug build
dotnet build -t:Run -f net10.0-windows10.0.19041.0
```

#### macOS

```bash
# Debug build
dotnet build -t:Run -f net10.0-maccatalyst
```

### Visual Studio

1. Open `BodyMeasurement.sln`
2. Select target framework from dropdown (e.g., `net10.0-android`)
3. Select device/emulator from device dropdown
4. Press F5 or click "Run"

## Production Builds

### Android APK/AAB

#### Signing Configuration

1. Generate keystore:

```bash
keytool -genkey -v -keystore bodymeasurement.keystore -alias bodymeasurement \
  -keyalg RSA -keysize 2048 -validity 10000
```

2. Add signing configuration to `BodyMeasurement.csproj`:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <AndroidSigningKeyStore>bodymeasurement.keystore</AndroidSigningKeyStore>
  <AndroidSigningKeyAlias>bodymeasurement</AndroidSigningKeyAlias>
  <AndroidSigningKeyPass>$(KEYSTORE_PASSWORD)</AndroidSigningKeyPass>
  <AndroidSigningStorePass>$(KEYSTORE_PASSWORD)</AndroidSigningStorePass>
</PropertyGroup>
```

3. Set environment variable:

```bash
export KEYSTORE_PASSWORD="your_password"
```

#### Build APK (for sideloading)

```bash
dotnet publish -f net10.0-android -c Release
```

Output: `BodyMeasurement/bin/Release/net10.0-android/publish/page.eugen.maui.ai.bodymeasurement-Signed.apk`

#### Build AAB (for Google Play)

```bash
dotnet publish -f net10.0-android -c Release -p:AndroidPackageFormat=aab
```

Output: `BodyMeasurement/bin/Release/net10.0-android/publish/page.eugen.maui.ai.bodymeasurement-Signed.aab`

#### Google Play Console Upload

1. Log in to [Google Play Console](https://play.google.com/console)
2. Create app or select existing app
3. Navigate to "Release" → "Production"
4. Create new release
5. Upload `.aab` file
6. Fill in release notes
7. Review and rollout

### iOS IPA

#### Provisioning Profile Setup

1. Create App ID in [Apple Developer Portal](https://developer.apple.com/)
   - Identifier: `page.eugen.maui.ai.bodymeasurement`
   - Enable capabilities as needed

2. Create Distribution Certificate
3. Create Distribution Provisioning Profile
4. Download and install profile

#### Build IPA

```bash
# Archive for App Store
dotnet publish -f net10.0-ios -c Release \
  -p:ArchiveOnBuild=true \
  -p:CodesignKey="Apple Distribution: Your Name (TEAM_ID)" \
  -p:CodesignProvision="Your Provisioning Profile Name"
```

Output: `BodyMeasurement/bin/Release/net10.0-ios/ios-arm64/publish/BodyMeasurement.ipa`

#### App Store Connect Upload

Option 1: Xcode (Recommended)

1. Open Xcode
2. Window → Organizer
3. Select your archive
4. Click "Distribute App"
5. Select "App Store Connect"
6. Follow wizard

Option 2: Transporter

1. Download [Transporter](https://apps.apple.com/app/transporter/id1450874784)
2. Open Transporter
3. Drag and drop `.ipa` file
4. Click "Deliver"

#### TestFlight

After upload to App Store Connect:

1. Log in to [App Store Connect](https://appstoreconnect.apple.com/)
2. Select your app
3. Go to "TestFlight" tab
4. Add internal testers
5. Submit for beta review (external testers)

### Windows MSIX

#### Certificate Setup

1. Generate self-signed certificate (for testing):

```powershell
New-SelfSignedCertificate -Type Custom -Subject "CN=Your Name" `
  -KeyUsage DigitalSignature -FriendlyName "Body Measurement Cert" `
  -CertStoreLocation "Cert:\CurrentUser\My" `
  -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")
```

2. Export certificate
3. Add to `BodyMeasurement.csproj`:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Release'">
  <PackageCertificateThumbprint>YOUR_CERT_THUMBPRINT</PackageCertificateThumbprint>
</PropertyGroup>
```

#### Build MSIX

```bash
dotnet publish -f net10.0-windows10.0.19041.0 -c Release \
  -p:GenerateAppxPackageOnBuild=true \
  -p:AppxPackageSigningEnabled=true
```

Output: `BodyMeasurement/bin/Release/net10.0-windows10.0.19041.0/win10-x64/AppPackages/`

#### Microsoft Store Upload

1. Create app in [Partner Center](https://partner.microsoft.com/dashboard)
2. Reserve app name
3. Navigate to "Packages"
4. Upload `.msix` or `.msixbundle` file
5. Fill in store listing details
6. Submit for certification

#### Sideloading (Enterprise)

Users can install MSIX directly:

1. Right-click MSIX file
2. Select "Install"
3. Or use PowerShell:

```powershell
Add-AppxPackage -Path .\BodyMeasurement.msix
```

### macOS APP

#### Code Signing

1. Create Mac App Distribution certificate in Apple Developer Portal
2. Create Mac App Store Provisioning Profile
3. Download and install

#### Build APP

```bash
dotnet publish -f net10.0-maccatalyst -c Release \
  -p:CodesignKey="3rd Party Mac Developer Application: Your Name (TEAM_ID)" \
  -p:CodesignProvision="Your Provisioning Profile Name" \
  -p:CodesignEntitlements="Platforms/MacCatalyst/Entitlements.plist"
```

Output: `BodyMeasurement/bin/Release/net10.0-maccatalyst/maccatalyst-arm64/publish/BodyMeasurement.app`

#### Mac App Store Upload

Same process as iOS using Transporter or Xcode Organizer.

## Version Management

### Updating Version Numbers

Edit `BodyMeasurement.csproj`:

```xml
<PropertyGroup>
  <ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>
  <ApplicationVersion>1</ApplicationVersion>
</PropertyGroup>
```

- `ApplicationDisplayVersion`: User-facing version (e.g., 1.0.0, 1.1.0)
- `ApplicationVersion`: Build number (must increment for each release)

### Platform-Specific Versions

#### Android

```xml
<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
  <ApplicationDisplayVersion>1.0.0</ApplicationDisplayVersion>
  <ApplicationVersion>1</ApplicationVersion>
  <!-- AndroidVersionCode must be integer and increase with each release -->
  <AndroidVersionCode>1</AndroidVersionCode>
</PropertyGroup>
```

#### iOS

```xml
<PropertyGroup Condition="'$(TargetFramework)' == 'net10.0-ios'">
  <CFBundleDisplayName>Body Measurement</CFBundleDisplayName>
  <CFBundleVersion>1.0.0</CFBundleVersion>
  <CFBundleShortVersionString>1.0.0</CFBundleShortVersionString>
</PropertyGroup>
```

## App Icons and Splash Screens

### Icons

Place icons in `BodyMeasurement/Resources/AppIcon/`:

- `appicon.svg` (vector, recommended)
- Or platform-specific sizes

MAUI automatically generates required sizes.

### Splash Screen

Place splash screen in `BodyMeasurement/Resources/Splash/`:

- `splash.svg` (vector, recommended)
- Or `splash.png` (1024x1024 minimum)

Configure in `BodyMeasurement.csproj`:

```xml
<MauiSplashScreen Include="Resources\Splash\splash.svg" Color="#FFFFFF" />
```

## Environment Configuration

### Debug vs Release

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Debug'">
  <DefineConstants>$(DefineConstants);DEBUG</DefineConstants>
</PropertyGroup>
```

Use in code:

```csharp
#if DEBUG
    Console.WriteLine("Debug mode");
#else
    Console.WriteLine("Release mode");
#endif
```

### Build Configurations

Create custom configurations:

```xml
<PropertyGroup Condition="'$(Configuration)' == 'Staging'">
  <DefineConstants>$(DefineConstants);STAGING</DefineConstants>
</PropertyGroup>
```

Build with custom configuration:

```bash
dotnet build -c Staging
```

## CI/CD Integration

### GitHub Actions

See `docs/ci-cd-setup.md` for detailed pipeline configuration.

### Azure DevOps

Example `azure-pipelines.yml`:

```yaml
trigger:
  - main

pool:
  vmImage: 'macOS-latest'

steps:
  - task: UseDotNet@2
    inputs:
      version: '10.x'
  
  - script: dotnet workload install maui
    displayName: 'Install MAUI workload'
  
  - script: dotnet build -c Release
    displayName: 'Build solution'
  
  - script: dotnet test
    displayName: 'Run tests'
  
  - script: dotnet publish -f net10.0-android -c Release
    displayName: 'Publish Android'
```

## Troubleshooting

### Android

**Issue**: Build fails with "Android SDK not found"

**Solution**: Install Android SDK via Visual Studio Installer or Android Studio

**Issue**: App crashes on startup

**Solution**: Check `Platforms/Android/AndroidManifest.xml` permissions

### iOS

**Issue**: Code signing failed

**Solution**: Verify provisioning profile matches App ID and certificate

**Issue**: App rejected by App Store

**Solution**: Check [App Store Review Guidelines](https://developer.apple.com/app-store/review/guidelines/)

### Windows

**Issue**: MSIX installation fails

**Solution**: Install certificate first, then install MSIX

### macOS

**Issue**: "App is damaged" error

**Solution**: Sign app with valid Developer ID certificate

## Release Checklist

- [ ] Update version numbers
- [ ] Update changelog
- [ ] Run all tests
- [ ] Build release for all platforms
- [ ] Test on physical devices
- [ ] Create release notes
- [ ] Tag release in Git
- [ ] Upload to app stores
- [ ] Submit for review
- [ ] Monitor crash reports after release

## Support

For platform-specific issues:
- [MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [Android Developer Guide](https://developer.android.com/)
- [iOS Developer Guide](https://developer.apple.com/ios/)
- [Windows App Development](https://learn.microsoft.com/windows/apps/)
