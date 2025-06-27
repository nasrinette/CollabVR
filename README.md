# Collaborative 3D Sketching in Mixed Reality

This project is a multi-user mixed reality (MR) application that enables real-time 3D sketching and spatial collaboration using Meta Quest devices. Users can draw in mid-air, adjust stroke properties, see each other via avatars, and export their work to standard 3D formats.

## 🎯 Features

- ✅ Real-time multi-user 3D drawing
- ✅ Avatar-based co-presence with head/hand tracking
- ✅ Passthrough rendering for physical-world context
- ✅ Stroke customization (color and width)
- ✅ Erasing tool for live editing
- ✅ OBJ export of all strokes
- ✅ Voice communication via Photon Voice


## 🎥 Demo Videos

Watch the demo here:  
[▶️ Project Demo Video](https://drive.google.com/drive/folders/1a2YOCi-Zk-2xqarRbOXCp0olZGsnpm_V?usp=sharing)

## 📦 Tech Stack

- Unity 2022.3
- Meta XR SDK (Passthrough, Avatars, Interaction)
- Photon Fusion 2 (networking)
- Photon Voice 2 (audio)
- Oculus XR Plugin
- Unity UI (UGUI)


## 🚀 Getting Started

1. Clone this repo:
   ```bash
   git clone https://github.com/nasrinette/CollabVR.git

2. Open the project in **Unity 2022.3** or later.

3. Set up **Photon**:

   * Create a free account at [Photon Engine Dashboard](https://dashboard.photonengine.com/).
   * Create two apps:

     * One for **Photon Fusion**
     * One for **Photon Voice**
   * Copy the App IDs.
   * In Unity, go to `Assets/Resources/PhotonAppSettings.asset` and paste the App IDs in the appropriate fields.

4. Set up **Meta Developer Account**:

   * Register at [Meta Developer Portal](https://developer.oculus.com/manage/).
   * Create a new app under your organization.
   * Enable the following permissions:

     * **Avatars**
     * **Passthrough**
     * **Voice**
     * **Hand tracking** (optional)
   * Copy your **Meta App ID**.
   * In Unity, go to:

     * `Project Settings → XR Plugin Management → Meta XR` and paste the App ID
     * `Assets/Resources/OculusPlatformSettings.asset` and update the App ID there too

5. Build and deploy to Meta Quest:

   * Connect your Quest headset via USB.
   * In Unity, go to `File → Build Settings → Android`, select the correct scene, and build.
   * Deploy using Android Build or SideQuest.

