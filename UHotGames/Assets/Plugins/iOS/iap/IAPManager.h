#import <Foundation/Foundation.h>
 #import <StoreKit/StoreKit.h>


 @interface IAPManager : NSObject<SKProductsRequestDelegate, SKPaymentTransactionObserver>{
     SKProduct *proUpgradeProduct;
     SKProductsRequest *productsRequest;
 }


 -(void)attachObserver;
 -(BOOL)CanMakePayment;
 -(void)requestProductData:(NSString *)productIdentifiers;
 -(void)buyRequest:(NSString *)productIdentifier;


 @end
