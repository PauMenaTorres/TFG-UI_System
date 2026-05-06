using System;
using System.Collections.Generic;

namespace ModularUIRuntime
{
    public interface IPlatformFactory
    {
        IPlatformUIAdapter CreateAdapter();
    }
}