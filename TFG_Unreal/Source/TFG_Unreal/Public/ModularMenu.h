#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "UITypes.h"
#include "ModularMenu.generated.h"

class UCanvasPanel;
class UGridPanel;
class UButton;
class UImage;

UCLASS()
class TFG_UNREAL_API UModularMenu : public UUserWidget
{
    GENERATED_BODY()

protected:

    virtual void NativePreConstruct() override;
    virtual void SynchronizeProperties() override;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Menu Config")
    TArray<FUIElementConfig> MenuElements;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Menu Config")
    UUITheme* MenuTheme = nullptr;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Menu Config")
    TEnumAsByte<EHorizontalAlignment> MenuHorizontalAlignment = HAlign_Center;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Menu Config")
    TEnumAsByte<EVerticalAlignment> MenuVerticalAlignment = VAlign_Center;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Menu Config")
    UTexture2D* BackgroundImage = nullptr;

    UPROPERTY(BlueprintReadWrite, meta = (BindWidgetOptional))
    UCanvasPanel* CanvasPanel = nullptr;

    UPROPERTY(BlueprintReadWrite, meta = (BindWidgetOptional))
    UImage* BackgroundWidget = nullptr;

    UPROPERTY(BlueprintReadWrite, meta = (BindWidgetOptional))
    UGridPanel* GridPanel = nullptr;

private:

    void BuildWidgetTree();
    void PopulateMenu();
    void SetupBackground();
    void AddToGrid(UWidget* Widget, const FUIElementConfig& Config, int32 AutoRow, FVector2D Size);

    void CreateButton(const FUIElementConfig& Config, int32 AutoRow);
    void CreateSlider(const FUIElementConfig& Config, int32 AutoRow);
    void CreateToggle(const FUIElementConfig& Config, int32 AutoRow);
    void CreateLabel(const FUIElementConfig& Config, int32 AutoRow);
    void CreateImage(const FUIElementConfig& Config, int32 AutoRow);
    void CreateSpacer(const FUIElementConfig& Config, int32 AutoRow);

    TMap<UButton*, FUIElementConfig> ButtonConfigMap;

    UFUNCTION()
    void OnDynamicButtonClicked();
};
