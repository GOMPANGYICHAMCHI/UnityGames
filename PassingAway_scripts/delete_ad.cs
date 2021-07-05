using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

 public class delete_ad : MonoBehaviour, IStoreListener
 {
	 [SerializeField]
	private UI_manager UM;
	private static IStoreController storeController;
    private static IExtensionProvider extensionProvider;
 
    #region 상품ID
    // 상품ID는 구글 개발자 콘솔에 등록한 상품ID와 동일하게 해주세요.
    public const string deleteAD = "delete_ad";

    #endregion
 
    void Start()
    {
        InitializePurchasing();
		UM.delete_ad.onClick.AddListener(BuyProductID);
    }
 
    private bool IsInitialized()
    {
        return (storeController != null && extensionProvider != null);
    }
 
    public void InitializePurchasing()
    {
        if (IsInitialized())
            return;
 
        var module = StandardPurchasingModule.Instance();
 
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
 
        builder.AddProduct(deleteAD, ProductType.NonConsumable, new IDs
        {
            { deleteAD, AppleAppStore.Name },
            { deleteAD, GooglePlay.Name },
        });
 
        UnityPurchasing.Initialize(this, builder);
    }
 
    public void BuyProductID()
    {
        try
        {
            if (IsInitialized())
            {
                Product p = storeController.products.WithID(deleteAD);
 
                if (p != null && p.availableToPurchase)
                {
                   // Debug.Log(string.Format("Purchasing product asychronously: '{0}'", p.definition.id));
                    storeController.InitiatePurchase(p);
                }
                else
                {
                    //purchase failed
                }
            }
            else
            {
                //purchase failed
            }
        }
        catch (Exception e)
        {
           	//purchase failed
        }
    }
 
    public void RestorePurchase()
    {
        if (!IsInitialized())
        {
            //restorepurchase failed
            return;
        }
 
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");
 
            var apple = extensionProvider.GetExtension<IAppleExtensions>();
 
            apple.RestoreTransactions
            (
                (result) => { Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); }
            );
        }
        else
        {
            //restorepurchase failed
        }
    }
 
    public void OnInitialized(IStoreController sc, IExtensionProvider ep)
    {
        storeController = sc;
        extensionProvider = ep;
    }
 
    public void OnInitializeFailed(InitializationFailureReason reason)
    {
        //initialize failed
    }
 
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
		//이곳에 구매했을때 광고제거
		PlayerPrefs.SetInt("AD_deleted",1);
		UM.ad_off = true;
        return PurchaseProcessingResult.Complete;
    }
 
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
       //purchase failed
    }
 }
       