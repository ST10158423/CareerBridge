using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.IO;

public class FirebaseAdminHelper
{
    private static bool _isInitialized;

    public static void InitializeFirebase()
    {
        if (!_isInitialized)
        {
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string serviceAccountPath = Path.Combine(rootPath, "careerbridge-d3523-firebase-adminsdk-orlai-92b77ab992.json");

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(serviceAccountPath)
            });

            _isInitialized = true;
        }
    }
}
