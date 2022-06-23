#region Access
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;
using Google.Play.Common;

#endregion
namespace MMA.Google_Play_Review
{
    public static class Key
    {

        // public const string _   = KeyData._;
        public static string Initialize = "GooglePlay_Review_Initialize";
        public static string DoReview = "GooglePlay_Review_DoReview";
        public static string OnError_ReviewOperation = "GooglePlay_Review_OnError_ReviewOperation";
        public static string OnSuccess_ReviewOperation = "GooglePlay_Review_OnSuccess_ReviewOperation";
    }
    public static class Import
    {
        //public const string _ = _;
    }
    public sealed partial class Google_Play_Review_Module : Module
    {
        #region References
        //[Header("Applications")]
        //[SerializeField] public ApplicationBase interface_Google_Play_Review;
        private ReviewManager manager_review = default;
        #endregion
        #region Reactions ( On___ )
        // Contenedor de toda las reacciones del Google_Play_Review
        protected override void OnSubscription(bool condition)
        {
            //Initialize
            Middleware.Subscribe_Publish(condition, Key.Initialize, Initialize);

            // DoReview
            Middleware.Subscribe_IEnumerator(condition, Key.DoReview, DoReview);
        }
        #endregion
        #region Methods
        // Contenedor de toda la logica del Google_Play_Review
        private void Initialize()
        {
            manager_review = new ReviewManager();
        }

        private bool CheckOperationHasError<T>(PlayAsyncOperation<T, ReviewErrorCode> playAsyncOperation)
        {
            if (playAsyncOperation.Error != ReviewErrorCode.NoError)
            {
                Debug.LogError(playAsyncOperation.Error.ToString());
                // ! OnError_ReviewOperation
                Middleware.Invoke_Publish(Key.OnError_ReviewOperation);
                return true;
            }
            return false;
        }
        #endregion
        #region Request ( Coroutines )
        // Contenedor de toda la Esperas de corutinas del Google_Play_Review

        private IEnumerator DoReview()
        {
            //TODO => esto se puede crear sub Ienumerator para que este retorne unicamente el bool ?

            //Crea y espera por la operación
            PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> playAsyncOperation_getInfo = manager_review.RequestReviewFlow();
            yield return playAsyncOperation_getInfo;

            //Check si hay error
            if (CheckOperationHasError(playAsyncOperation_getInfo)) yield break;

            //Review Info
            PlayAsyncOperation<VoidResult, ReviewErrorCode> playAsyncOperation_launchReview = manager_review.LaunchReviewFlow(playAsyncOperation_getInfo.GetResult());
            yield return playAsyncOperation_launchReview;

            //Revisa si hay errores
            if (CheckOperationHasError(playAsyncOperation_getInfo)) yield break;

            // ! Success
            Middleware.Invoke_Publish(Key.OnSuccess_ReviewOperation);
        }

        #endregion
        #region Task ( async )
        // Contenedor de toda la Esperas asincronas del Google_Play_Review
        #endregion
    }
}