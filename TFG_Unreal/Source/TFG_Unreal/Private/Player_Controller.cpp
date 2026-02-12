#include "Player_Controller.h"
#include "Blueprint/UserWidget.h"

void APlayer_Controller::BeginPlay()
{
    Super::BeginPlay();

    UE_LOG(LogTemp, Warning, TEXT("Player_Controller BeginPlay"));

    UClass* WidgetClass = LoadClass<UUserWidget>(nullptr, TEXT("/Game/UI/ModularMenu.ModularMenu_C"));
    
    if(WidgetClass)
    {
        UE_LOG(LogTemp, Warning, TEXT("ModularMenu class loaded"));
        MainMenuWidget = CreateWidget<UUserWidget>(this, WidgetClass);
        
        if(MainMenuWidget)
        {
            UE_LOG(LogTemp, Warning, TEXT("ModularMenu created. Adding to viewport..."));
            MainMenuWidget->AddToViewport(0);
            
            bShowMouseCursor = true;
            SetInputMode(FInputModeUIOnly());
            UE_LOG(LogTemp, Warning, TEXT("Done!"));
        }
        else
        {
            UE_LOG(LogTemp, Warning, TEXT("Failed to create ModularMenu instance"));
        }
    }
    else
    {
        UE_LOG(LogTemp, Warning, TEXT("Failed to load ModularMenu class"));
    }
}
