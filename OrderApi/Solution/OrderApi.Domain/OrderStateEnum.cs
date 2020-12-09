using System;

namespace OrderApi.Domain
{
    [Flags]
    public enum OrderState
    {
        PaidOrder = 1,
        NotPaidOrder = 2
    }
}
