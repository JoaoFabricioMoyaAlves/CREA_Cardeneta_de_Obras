import { useState } from "react";
import { Button } from "@/components/ui/button";
import { MessageSquareText } from "lucide-react";
import { AssistentePanel } from "./AssistentePanel";

export function AssistenteButton() {
  const [open, setOpen] = useState(false);
  return (
    <>
      <Button
        aria-label="Abrir assistente de IA"
        onClick={() => setOpen(true)}
        className="fixed right-5 bottom-5 z-40 size-14 rounded-full bg-primary p-0 shadow-lg hover:bg-primary/90"
      >
        <MessageSquareText className="size-6" />
      </Button>
      <AssistentePanel open={open} onOpenChange={setOpen} />
    </>
  );
}
