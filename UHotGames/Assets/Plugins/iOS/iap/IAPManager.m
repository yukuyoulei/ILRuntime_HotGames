#import "IAPManager.h"

 @implementation IAPManager


 -(void) attachObserver{
     NSLog(@"AttachObserver");
     [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
 }


 -(BOOL) CanMakePayment{
     return [SKPaymentQueue canMakePayments];
 }


 -(void) requestProductData:(NSString *)productIdentifiers{
     NSArray *idArray = [productIdentifiers componentsSeparatedByString:@"\t"];
     NSSet *idSet = [NSSet setWithArray:idArray];
     [self sendRequest:idSet];
 }


 -(void)sendRequest:(NSSet *)idSet{
     SKProductsRequest *request = [[SKProductsRequest alloc] initWithProductIdentifiers:idSet];
     request.delegate = self;
     [request start];
 }


 -(void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response{
     NSArray *products = response.products;
     
     for (SKProduct *p in products) {
         UnitySendMessage("Main", "ShowProductList", [[self productInfo:p] UTF8String]);
     }
     
     for(NSString *invalidProductId in response.invalidProductIdentifiers){
         NSLog(@"Invalid product id:%@",invalidProductId);
     }
     
     //[request autorelease];
 }


 -(void)buyRequest:(NSString *)productIdentifier{
     SKPayment *payment = [SKPayment paymentWithProductIdentifier:productIdentifier];
     [[SKPaymentQueue defaultQueue] addPayment:payment];
 }


 -(NSString *)productInfo:(SKProduct *)product{
     NSArray *info = [NSArray arrayWithObjects:product.localizedTitle,product.localizedDescription,product.price,product.productIdentifier, nil];
     
     return [info componentsJoinedByString:@"\t"];
 }


 -(NSString *)transactionInfo:(SKPaymentTransaction *)transaction{
     
     return [self encode:(uint8_t *)transaction.transactionReceipt.bytes length:transaction.transactionReceipt.length];
     
     //return [[NSString alloc] initWithData:transaction.transactionReceipt encoding:NSASCIIStringEncoding];
 }


 -(NSString *)encode:(const uint8_t *)input length:(NSInteger) length{
     static char table[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
     
     NSMutableData *data = [NSMutableData dataWithLength:((length+2)/3)*4];
     uint8_t *output = (uint8_t *)data.mutableBytes;
     
     for(NSInteger i=0; i<length; i+=3){
         NSInteger value = 0;
         for (NSInteger j= i; j<(i+3); j++) {
             value<<=8;
             
             if(j<length){
                 value |=(0xff & input[j]);
             }
         }
         
         NSInteger index = (i/3)*4;
         output[index + 0] = table[(value>>18) & 0x3f];
         output[index + 1] = table[(value>>12) & 0x3f];
         output[index + 2] = (i+1)<length ? table[(value>>6) & 0x3f] : '=';
         output[index + 3] = (i+2)<length ? table[(value>>0) & 0x3f] : '=';
     }
     
     return [[NSString alloc] initWithData:data encoding:NSASCIIStringEncoding];
 }


 -(void) provideContent:(SKPaymentTransaction *)transaction{
     UnitySendMessage("SDK_AppleInApp", "ProvideContent", [[self transactionInfo:transaction] UTF8String]);
 }


 -(void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions{
     for (SKPaymentTransaction *transaction in transactions) {
         switch (transaction.transactionState) {
             case SKPaymentTransactionStatePurchased:
                 [self completeTransaction:transaction];
                 break;
             case SKPaymentTransactionStateFailed:
                 [self failedTransaction:transaction];
				 UnitySendMessage("SDK_AppleInApp", "UpdateTransactions", "");
                 break;
             case SKPaymentTransactionStateRestored:
                 [self restoreTransaction:transaction];
				 UnitySendMessage("SDK_AppleInApp", "UpdateTransactions", "");
                 break;
             default:
                 break;
         }
     }
 }


 -(void) completeTransaction:(SKPaymentTransaction *)transaction{
     NSLog(@"Comblete transaction : %@",transaction.transactionIdentifier);
     [self provideContent:transaction];
     [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
 }


 -(void) failedTransaction:(SKPaymentTransaction *)transaction{
     NSLog(@"Failed transaction : %@",transaction.transactionIdentifier);
     
     if (transaction.error.code != SKErrorPaymentCancelled) {
         NSLog(@"!Cancelled");
     }
     [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
 }


 -(void) restoreTransaction:(SKPaymentTransaction *)transaction{
     NSLog(@"Restore transaction : %@",transaction.transactionIdentifier);
     [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
 }




 @end
