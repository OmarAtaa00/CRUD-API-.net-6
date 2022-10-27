using System;
using ErrorOr;

namespace BreakfastMaker.ServiceError;

public static class Errors
{
    public static class Breakfast
    {
        public static Error NotFound => Error.NotFound(
            code: "Breakfast.NoteFound",
            description: "Breakfast not found "
        );
    }

}
