#include <mach/mach.h>

// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
	if (string)
		return [NSString stringWithUTF8String: string];
	else
		return [NSString stringWithUTF8String: ""];
}

// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
	if (string == NULL)
		return NULL;
	
	char* res = (char*)malloc(strlen(string) + 1);
	strcpy(res, string);
	return res;
}

BOOL memoryInfo(vm_statistics_data_t *vmStats) { 
    mach_msg_type_number_t infoCount = HOST_VM_INFO_COUNT; 
    kern_return_t kernReturn = host_statistics(mach_host_self(), HOST_VM_INFO, (host_info_t)vmStats, &infoCount); 
     
    return kernReturn == KERN_SUCCESS; 
} 
 
void logMemoryInfo() { 
    vm_statistics_data_t vmStats; 
     
    if (memoryInfo(&vmStats)) { 
        NSLog(@"free: %u\nactive: %u\ninactive: %u\nwire: %u\nzero fill: %u\nreactivations: %u\npageins: %u\npageouts: %u\nfaults: %u\ncow_faults: %u\nlookups: %u\nhits: %u", 
            vmStats.free_count * vm_page_size, 
            vmStats.active_count * vm_page_size, 
            vmStats.inactive_count * vm_page_size, 
            vmStats.wire_count * vm_page_size, 
            vmStats.zero_fill_count * vm_page_size, 
            vmStats.reactivations * vm_page_size, 
            vmStats.pageins * vm_page_size, 
            vmStats.pageouts * vm_page_size, 
            vmStats.faults, 
            vmStats.cow_faults, 
            vmStats.lookups, 
            vmStats.hits 
        ); 
    } 
}

double getFreeMemory()
{
    vm_statistics_data_t vmStats;
    if (memoryInfo(&vmStats))
    {
        return vmStats.free_count * vm_page_size / 1024.0 / 1024.0;
    }
    return 0;
}

void GeekWriteFile(NSString* sPathFrom, NSString* sPathTo)
{
    NSString *fileRead = sPathFrom;
    NSString *fileWrite = sPathTo;
    FILE *fpr = NULL, *fpw = NULL;
    fpr = fopen([fileRead UTF8String], "rb");
    fpw = fopen([fileWrite UTF8String], "wb+");
    if (fpr != NULL && fpw != NULL)
    {
        while (!feof(fpr))
        {
            fputc(fgetc(fpr), fpw);
        }
        fflush(fpw);
        fclose(fpr);
        fclose(fpw);
            
            
    } 
}

void OnExit()
{
	exit(0);
}

void GeekWriteFile(Byte* bytes, NSString* fileName, int byteCount)
{
    NSString *fileWrite = [[[NSBundle mainBundle] resourcePath] stringByAppendingPathComponent:fileName];
    FILE *fpw = NULL;
    fpw = fopen([fileWrite UTF8String], "wb+");
    if (fpw != NULL)
    {
        fwrite(bytes, sizeof(Byte), byteCount, fpw);
        
        fflush(fpw);
        
        fclose(fpw);
        
        
    }
}

UIAlertView* view = nil;
void waitBegin(NSString* title)
{
	if (view != nil)
	{
		[view setTitle:title];
		[view show];
		return;
	}

    view = [[UIAlertView alloc]initWithTitle:title
                        message:nil
                        delegate:nil
                        cancelButtonTitle:nil
                        otherButtonTitles:nil,
                        nil];    [view show];
}

void waitEnd()
{
    if (view == nil)
    {
        return;
    }
	[view dismissWithClickedButtonIndex:0 animated:YES];
}

extern "C"
{	void GeekWriteFile(Byte* bytes, const char* fileName, int byteCount)
    {
        GeekWriteFile(bytes, CreateNSString(fileName), byteCount);
    }

    void GeekWriteFileByName(const char* fileNameFrom, const char* fileNameTo)
    {
        GeekWriteFile(CreateNSString(fileNameFrom), CreateNSString(fileNameTo));
    }    double GetFreeMemory()
    {
        return getFreeMemory();
    }
	
	double GetUsedMemory()
	{
	  task_basic_info_data_t taskInfo;
	  mach_msg_type_number_t infoCount = TASK_BASIC_INFO_COUNT;
	  kern_return_t kernReturn = task_info(mach_task_self(), 
										   TASK_BASIC_INFO, 
										   (task_info_t)&taskInfo, 
										   &infoCount);

	  if (kernReturn != KERN_SUCCESS
		  ) {
		return NSNotFound;
	  }
	  
	  return taskInfo.resident_size / 1024.0 / 1024.0;
	}
	
	const char* GetBundleVersion()
	{
		NSString *version = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleVersion"];
		return MakeStringCopy([version UTF8String]);
	}
	const char* GetVersionNumber()
	{
		NSString *version = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"CFBundleShortVersionString"];
		return MakeStringCopy([version UTF8String]);
	}

	void Exit()
	{
		exit(0);
	}

	void OnWaitBegin(const char* message)
	{
		waitBegin(CreateNSString(message));
	}
	void OnWaitEnd()
	{
		waitEnd();
	}
}
