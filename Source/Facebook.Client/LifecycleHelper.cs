﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using System.Net;
using Facebook;
using Facebook.Client.Controls;

namespace Facebook.Client
{
    public class LifecycleHelper
    {

        public static DismissDialogDelegate OnDialogDismissed;
#if WP8
        
#endif

#if WINDOWS_UNIVERSAL
        public static void FacebookAuthenticationReceived(ProtocolActivatedEventArgs protocolArgs)
        {
            if (protocolArgs == null)
            {
                throw new ArgumentNullException("protocolArgs");
            }

            // If this invocation is because of a dialog dismissal, dismiss the dialog
            if (OnDialogDismissed != null)
            {
                OnDialogDismissed(protocolArgs.Uri);
            }


            // parse and fill out the token data
            try
            {
                AccessTokenData tokenData = new AccessTokenData();
                tokenData.ParseQueryString(Facebook.HttpHelper.UrlDecode(protocolArgs.Uri.ToString()));
                if (!String.IsNullOrEmpty(tokenData.AccessToken))
                {
                    var task = Task.Run(async () => await AppAuthenticationHelper.GetFacebookConfigValue("Facebook", "AppId"));
                    task.Wait();
                    tokenData.AppId = task.Result;
                    Session.ActiveSession.CurrentAccessTokenData = tokenData;

                    // trigger the event handler with the session
                    if (Session.OnFacebookAuthenticationFinished != null)
                    {
                        Session.OnFacebookAuthenticationFinished(tokenData);
                    }

                    if (Session.OnSessionStateChanged != null)
                    {
                        Session.OnSessionStateChanged(LoginStatus.LoggedIn);
                    }
                }
            }
            catch (Facebook.FacebookOAuthException exc)
            {
                  // TODO: (sanjeevd) catch appropriately
            }
        }
#endif
    }
}
