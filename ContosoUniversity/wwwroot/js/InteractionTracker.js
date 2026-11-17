//Will track hovering, page lingering, 
// use strict prevents interference with other js
(() => {
    "use strict";
    const MIN_HOVER_MS = 1000;
    const MAX_HOVER_MS = 20000;
    const MAX_PAGE_MS = 40000;
    const STORAGE_KEY = "BugBeadingInteractionRecords";
    let activeHover = null;

    //Page duration tracking
    let pageStart =
        document.visibilityState === "visible" ? Date.now() : null;
    let pageElapsed = 0;
    let pageRecorded = false;

    //Load records saved during this browser tab's session
    const records = (() => {
        try {
            return JSON.parse(
                sessionStorage.getItem(STORAGE_KEY) || "[]"
            );
        } catch (error) {
            console.warn("Unable to load tracking data:", error);
            return [];
        }
    })();

    //Identifies tracked elements
    function getTrackedElement(target) {
        if (!(target instanceof Element)) return null;
        const image = target.closest("[data-track-image]");
        if (image) {
            return {
                element: image,
                type: "image"
            };
        }
        const tracked = target.closest("[data-track]");
        if (tracked) {
            return {
                element: tracked,
                type: "element"
            };
        }
        return null;
    }

    //Saves records so they survive between page navigation
    function saveRecords() {
        try {
            sessionStorage.setItem(
                STORAGE_KEY,
                JSON.stringify(records)
            );
        } catch (error) {
            console.warn("Unable to save tracking data:", error);
        }
    }

    //Records interactions
    function recordInteraction(type, element, duration = 0) {
        const productElement = element?.closest("[data-product-id]");
        const record = {
            interactionType: type,
            elementType: element?.dataset?.track ||
                (type.startsWith("image") ? "image" : "page"),
            productId: element?.dataset?.productId ||
                productElement?.dataset?.productId ||
                null,
            durationMilliseconds: Math.round(duration),
            page: window.location.pathname,
            timestamp: new Date().toISOString()
        };
        records.push(record);
        saveRecords();
        console.log("[Bug Beading Tracking]", record);
    }

    //Finishes current hover
    function finishHover() {
        if (!activeHover) return;
        clearTimeout(activeHover.qualificationTimeout);
        clearTimeout(activeHover.maxTimeout);
        const duration = Math.min(
            Date.now() - activeHover.startedAt,
            MAX_HOVER_MS
        );
        //Only record a hover if the mouse was still for at least one second before leaving or timing out.
        if (activeHover.qualified) {
            recordInteraction(
                activeHover.type,
                activeHover.element,
                duration
            );
        }
        activeHover = null;
    }

    //Starts a new hover
    function startHover(element, type) {
        finishHover();
        activeHover = {
            element,
            type,
            startedAt: Date.now(),
            qualified: false,
            qualificationTimeout: null,
            maxTimeout: null
        };
        //The mouse must stop for one second
        activeHover.qualificationTimeout = setTimeout(() => {
            if (activeHover) {
                activeHover.qualified = true;
            }
        }, MIN_HOVER_MS);
        //Limits individual hover sessions
        activeHover.maxTimeout = setTimeout(
            finishHover,
            MAX_HOVER_MS
        );
    }

    //Detects when the mouse enters a tracked element
    document.addEventListener("pointerover", event => {
        if (event.pointerType !== "mouse") return;
        const tracked = getTrackedElement(event.target);
        if (!tracked) return;
        const related = getTrackedElement(event.relatedTarget);
        //Ignore movement in same tracked element
        if (related?.element === tracked.element) return;
        startHover(
            tracked.element,
            tracked.type === "image" ? "image_hover" : "hover"
        );
    });

    //Resets one-second qualification timer whenever the mouse moves, including in the same element
    document.addEventListener("pointermove", event => {
        if (event.pointerType !== "mouse") return;
        if (!activeHover) return;
        activeHover.qualified = false;
        clearTimeout(activeHover.qualificationTimeout);
        activeHover.qualificationTimeout = setTimeout(() => {
            if (activeHover) {
                activeHover.qualified = true;
            }
        }, MIN_HOVER_MS);
    });

    //Detects when the mouse leaves tracked element
    document.addEventListener("pointerout", event => {
        if (event.pointerType !== "mouse") return;
        if (!activeHover) return;
        const current = getTrackedElement(event.target);
        const related = getTrackedElement(event.relatedTarget);
        if (
            current?.element === activeHover.element &&
            related?.element !== activeHover.element
        ) {
            finishHover();
        }
    });

    //Tracks clicks on tracked elements
    document.addEventListener("click", event => {
        const tracked = getTrackedElement(event.target);
        if (!tracked) return;
        recordInteraction(
            tracked.type === "image" ? "image_click" : "click",
            tracked.element
        );
    });

    //Accumulates visible page time
    function accumulatePageTime() {
        if (pageStart === null) return;

        pageElapsed += Date.now() - pageStart;
        pageStart = null;
    }

    //Records total page duration
    function finishPage() {
        if (pageRecorded) return;
        accumulatePageTime();
        if (pageElapsed > 0) {
            recordInteraction(
                "page_duration",
                null,
                Math.min(pageElapsed, MAX_PAGE_MS)
            );
        }
        pageRecorded = true;
    }

    //Handles browser tab visibility
    document.addEventListener("visibilitychange", () => {
        if (document.visibilityState === "hidden") {
            accumulatePageTime();
            finishHover();
        } else if (pageStart === null && !pageRecorded) {
            pageStart = Date.now();
        }
    });
    //Handles navigation and browser closure
    window.addEventListener("pagehide", () => {
        finishPage();
        finishHover();
    });
    //Exposes recorded interactions
    window.BugBeadingTracker = {
        getRecords: () => [...records]
    };
    console.log("Bug Beading interaction tracker initialized.");
})();