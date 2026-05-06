const clamp = (value, min = 0, max = 1) => Math.min(Math.max(value, min), max);
const easeInOutCubic = (value) => value < 0.5
    ? 4 * value * value * value
    : 1 - Math.pow(-2 * value + 2, 3) / 2;

const bounceEase = (value) => {
    const c1 = 1.70158;
    const c2 = c1 * 1.525;

    return value < 0.5
        ? (Math.pow(2 * value, 2) * ((c2 + 1) * 2 * value - c2)) / 2
        : (Math.pow(2 * value - 2, 2) * ((c2 + 1) * (value * 2 - 2) + c2) + 2) / 2;
};

const initMountainRange = async () => {
    const canvas = document.querySelector('[data-mountain-range]');

    if (!canvas || window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
        return;
    }

    let THREE;

    try {
        THREE = await import('/lib/three/three.module.js');
    } catch {
        canvas.hidden = true;
        return;
    }

    const renderer = new THREE.WebGLRenderer({ canvas, alpha: true, antialias: true });
    const scene = new THREE.Scene();
    const camera = new THREE.OrthographicCamera(-10, 10, 5, -5, 0.1, 100);
    const range = new THREE.Group();
    const layerColors = ['#154E4E', '#1F6E6E', '#C4614A'];

    camera.position.z = 10;
    scene.add(range);

    const makeLayer = (index, yOffset, height, color) => {
        const shape = new THREE.Shape();
        const points = [
            [-11, -5],
            [-11, yOffset],
            [-8.5, yOffset + height * 0.55],
            [-6.7, yOffset + height * 0.22],
            [-4.8, yOffset + height * 0.8],
            [-2.2, yOffset + height * 0.28],
            [0.4, yOffset + height],
            [2.5, yOffset + height * 0.35],
            [5.2, yOffset + height * 0.72],
            [7.2, yOffset + height * 0.3],
            [10.5, yOffset + height * 0.62],
            [11, yOffset],
            [11, -5]
        ];

        points.forEach(([x, y], pointIndex) => {
            if (pointIndex === 0) {
                shape.moveTo(x, y);
            } else {
                shape.lineTo(x, y);
            }
        });

        const geometry = new THREE.ShapeGeometry(shape);
        const material = new THREE.MeshBasicMaterial({
            color,
            transparent: true,
            opacity: 0.16 + index * 0.08,
            depthWrite: false
        });
        const mesh = new THREE.Mesh(geometry, material);

        mesh.position.z = -index * 0.1;
        mesh.position.y = -index * 0.42;
        range.add(mesh);
    };

    layerColors.forEach((color, index) => makeLayer(index, -2.9 + index * 0.32, 1.2 + index * 0.28, color));

    const resize = () => {
        const rect = canvas.getBoundingClientRect();
        const width = Math.max(rect.width, 1);
        const height = Math.max(rect.height, 1);

        renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
        renderer.setSize(width, height, false);
        camera.left = -10;
        camera.right = 10;
        camera.top = 10 * (height / width);
        camera.bottom = -10 * (height / width);
        camera.updateProjectionMatrix();
    };

    const update = () => {
        const progress = clamp(window.scrollY / 300);
        const blurProgress = progress < 0.5 ? progress * 2 : (1 - progress) * 2;
        const slowScroll = window.scrollY * 0.3;

        canvas.style.opacity = String(0.2 + progress * 0.6);
        canvas.style.filter = `sepia(${1 - progress}) blur(${2 * blurProgress}px)`;
        canvas.style.transform = `translate3d(0, ${-slowScroll}px, 0)`;
        range.position.x = (progress - 0.5) * 0.7;
        range.position.y = progress * 0.18;

        renderer.render(scene, camera);
        requestAnimationFrame(update);
    };

    resize();
    window.addEventListener('resize', resize);
    update();
};

const initDestinationCinema = () => {
    const section = document.querySelector('[data-destination-cinema]');
    const rail = document.querySelector('[data-card-rail]');

    if (!section || !rail) {
        return;
    }

    const cards = [...rail.querySelectorAll('[data-destination-card]')];
    const imageFrames = [...rail.querySelectorAll('[data-cinematic-image]')];
    let lastScrollLeft = rail.scrollLeft;
    let snapTimer;

    const getProgress = () => {
        const maxScroll = Math.max(rail.scrollWidth - rail.clientWidth, 1);
        return clamp(rail.scrollLeft / maxScroll);
    };

    const snapToNearestCard = () => {
        const railCenter = rail.scrollLeft + rail.clientWidth / 2;
        const nearest = cards.reduce((current, card) => {
            const cardCenter = card.offsetLeft + card.offsetWidth / 2;
            const distance = Math.abs(cardCenter - railCenter);

            return distance < current.distance ? { distance, cardCenter } : current;
        }, { distance: Infinity, cardCenter: cards[0]?.offsetWidth / 2 || 0 });

        rail.scrollTo({
            left: nearest.cardCenter - rail.clientWidth / 2,
            behavior: 'smooth'
        });
    };

    const revealImages = () => {
        const viewportHeight = window.innerHeight;

        imageFrames.forEach((frame) => {
            const rect = frame.getBoundingClientRect();
            const triggerStart = viewportHeight + 100;
            const triggerEnd = viewportHeight - 200;
            const progress = clamp((triggerStart - rect.top) / Math.max(triggerStart - triggerEnd, 1));
            const eased = easeInOutCubic(progress);

            if (progress > 0.05) {
                frame.classList.add('is-revealed');
            }

            frame.style.setProperty('--sg-image-scale', String(1 + eased * 0.25));
            frame.style.setProperty('--sg-image-pan-x', `${eased * 5}%`);
            frame.style.setProperty('--sg-image-pan-y', `${50 + eased * 3}%`);
        });
    };

    const updateCards = () => {
        const direction = Math.sign(rail.scrollLeft - lastScrollLeft);
        const progress = getProgress();
        const easedProgress = clamp(bounceEase(progress));
        const cameraDegrees = -20 + easedProgress * 40;

        section.style.setProperty('--sg-card-pan', `${50 + cameraDegrees * 0.9}%`);

        cards.forEach((card) => {
            const railCenter = rail.scrollLeft + rail.clientWidth / 2;
            const cardCenter = card.offsetLeft + card.offsetWidth / 2;
            const distance = Math.abs(cardCenter - railCenter);
            const normalized = clamp(distance / Math.max(card.offsetWidth * 1.2, 1));
            const active = 1 - normalized;
            const tilt = (direction || Math.sign(cardCenter - railCenter) || 1) * (3 + normalized * 2);

            card.style.setProperty('--sg-card-opacity', String(0.4 + active * 0.6));
            card.style.setProperty('--sg-card-tilt', `${tilt}deg`);
            card.style.setProperty('--sg-card-skew', `${(direction || 1) * 1.5 * active}deg`);
            card.style.boxShadow = `0 ${10 + active * 10}px ${20 + active * 20}px rgba(26,26,24,${0.12 + active * 0.08})`;
        });

        lastScrollLeft = rail.scrollLeft;
        revealImages();
    };

    rail.addEventListener('scroll', () => {
        updateCards();
        window.clearTimeout(snapTimer);
        snapTimer = window.setTimeout(snapToNearestCard, 300);
    }, { passive: true });

    rail.addEventListener('wheel', (event) => {
        const canScrollHorizontally = rail.scrollWidth > rail.clientWidth;
        const sectionRect = section.getBoundingClientRect();
        const isInsideSection = sectionRect.top < window.innerHeight * 0.72 && sectionRect.bottom > window.innerHeight * 0.28;

        if (!canScrollHorizontally || !isInsideSection || Math.abs(event.deltaX) > Math.abs(event.deltaY)) {
            return;
        }

        const atStart = rail.scrollLeft <= 0 && event.deltaY < 0;
        const atEnd = rail.scrollLeft >= rail.scrollWidth - rail.clientWidth - 1 && event.deltaY > 0;

        if (atStart || atEnd) {
            return;
        }

        event.preventDefault();
        rail.scrollLeft += event.deltaY;
    }, { passive: false });

    window.addEventListener('scroll', revealImages, { passive: true });
    window.addEventListener('resize', updateCards);
    updateCards();
};

const initLandingPage = () => {
    const revealItems = [...document.querySelectorAll('.lp-reveal')];
    const counters = [...document.querySelectorAll('[data-count]')];
    const timeline = document.querySelector('[data-timeline]');
    const accordions = [...document.querySelectorAll('[data-accordion] .lp-faq-item')];

    if (!revealItems.length && !counters.length && !accordions.length) {
        return;
    }

    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const countUp = (element) => {
        if (element.dataset.counted === 'true') {
            return;
        }

        const target = Number(element.dataset.count || '0');
        const duration = prefersReducedMotion ? 0 : 900;
        const start = performance.now();

        element.dataset.counted = 'true';

        const tick = (now) => {
            const progress = duration === 0 ? 1 : clamp((now - start) / duration);
            const eased = 1 - Math.pow(1 - progress, 4);
            const value = Math.round(target * eased);

            element.textContent = value.toLocaleString();

            if (progress < 1) {
                requestAnimationFrame(tick);
            }
        };

        requestAnimationFrame(tick);
    };

    const revealObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (!entry.isIntersecting) {
                return;
            }

            entry.target.classList.add('is-visible');
            revealObserver.unobserve(entry.target);
        });
    }, {
        threshold: 0.16,
        rootMargin: '0px 0px -8% 0px'
    });

    revealItems.forEach((item, index) => {
        item.style.transitionDelay = `${Math.min(index % 5, 4) * 90}ms`;
        revealObserver.observe(item);
    });

    const counterObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (!entry.isIntersecting) {
                return;
            }

            countUp(entry.target);
            counterObserver.unobserve(entry.target);
        });
    }, { threshold: 0.35 });

    counters.forEach((counter) => counterObserver.observe(counter));

    if (timeline) {
        const updateTimeline = () => {
            const rect = timeline.getBoundingClientRect();
            const viewport = window.innerHeight;
            const progress = clamp((viewport * 0.82 - rect.top) / Math.max(rect.height, 1));

            timeline.style.setProperty('--timeline-progress', `${progress * 100}%`);
        };

        window.addEventListener('scroll', updateTimeline, { passive: true });
        window.addEventListener('resize', updateTimeline);
        updateTimeline();
    }

    accordions.forEach((item, index) => {
        const button = item.querySelector('button');

        if (!button) {
            return;
        }

        if (index === 0) {
            item.classList.add('is-open');
            button.setAttribute('aria-expanded', 'true');
        }

        button.addEventListener('click', () => {
            const willOpen = !item.classList.contains('is-open');

            accordions.forEach((other) => {
                other.classList.remove('is-open');
                other.querySelector('button')?.setAttribute('aria-expanded', 'false');
            });

            if (willOpen) {
                item.classList.add('is-open');
                button.setAttribute('aria-expanded', 'true');
            }
        });
    });
};

const initBookingFlow = () => {
    const flow = document.querySelector('[data-booking-flow]');

    if (!flow) {
        return;
    }

    const stages = [...flow.querySelectorAll('[data-stage]')];
    const progressSteps = [...flow.querySelectorAll('[data-progress-step]')];
    const stageButtons = [...flow.querySelectorAll('[data-next-stage]')];
    const swapButtons = [...flow.querySelectorAll('[data-swap-button]')];
    const payButtons = [...flow.querySelectorAll('[data-pay-method]')];
    const checkoutForm = flow.querySelector('[data-checkout-form]');
    const alternatives = [
        'Mactan Ceramic Courtyard',
        'Liloan Moon Tide Table',
        'Busay Garden Hideout',
        'Alcoy White Rock Swim',
        'Kamagayan Vinyl Supper'
    ];

    const setStage = (stageName) => {
        stages.forEach((stage) => {
            const isActive = stage.dataset.stage === stageName;

            stage.classList.toggle('is-active', isActive);
            stage.setAttribute('aria-hidden', String(!isActive));
        });

        progressSteps.forEach((step) => {
            step.classList.toggle('is-active', step.dataset.progressStep === stageName);
        });

        flow.scrollIntoView({ behavior: 'smooth', block: 'start' });
    };

    stageButtons.forEach((button) => {
        button.addEventListener('click', () => setStage(button.dataset.nextStage));
    });

    swapButtons.forEach((button, index) => {
        button.addEventListener('click', () => {
            const item = button.closest('[data-journey-item]');
            const title = item?.querySelector('[data-item-title]');
            const note = item?.querySelector('[data-swap-note]');
            const nextTitle = alternatives[(index + Number(button.dataset.swapCount || '0')) % alternatives.length];

            button.dataset.swapCount = String(Number(button.dataset.swapCount || '0') + 1);

            if (title) {
                title.textContent = nextTitle;
            }

            if (note) {
                note.textContent = 'Swapped. AI found a quieter gem with a similar comfort profile.';
            }
        });
    });

    payButtons.forEach((button) => {
        button.addEventListener('click', () => {
            button.textContent = 'Authorizing...';
            window.setTimeout(() => setStage('success'), 520);
        });
    });

    checkoutForm?.addEventListener('submit', (event) => {
        event.preventDefault();
        setStage('success');
    });
};

const initAccountFlow = () => {
    const emailForm = document.querySelector('[data-auth-email-form]');

    emailForm?.addEventListener('submit', () => {
        const button = emailForm.querySelector('.auth-primary');

        if (!button) {
            return;
        }

        button.classList.add('is-loading');
        button.textContent = 'Checking account...';
    });
};

const initDashboard = () => {
    const dashboard = document.querySelector('[data-dashboard]');

    if (!dashboard) {
        return;
    }

    const vibeTags = [...dashboard.querySelectorAll('[data-vibe-tag]')];
    const surpriseButton = dashboard.querySelector('[data-surprise-button]');
    const surprisePanel = dashboard.querySelector('[data-surprise-panel]');
    const surpriseTitle = dashboard.querySelector('[data-surprise-title]');
    const surpriseReason = dashboard.querySelector('[data-surprise-reason]');
    const gems = [...dashboard.querySelectorAll('[data-surprise-gem]')];

    vibeTags.forEach((tag) => {
        tag.addEventListener('click', () => {
            tag.classList.toggle('is-active');
        });
    });

    surpriseButton?.addEventListener('click', () => {
        if (!gems.length || !surprisePanel || !surpriseTitle || !surpriseReason) {
            return;
        }

        const index = Math.floor(Math.random() * gems.length);
        const gem = gems[index];

        gems.forEach((item) => item.classList.remove('is-featured'));
        gem.classList.add('is-featured');
        surpriseTitle.textContent = gem.dataset.title || 'A hidden Cebu gem';
        surpriseReason.textContent = gem.dataset.reason || 'Matched to your travel profile.';
        surprisePanel.hidden = false;
        surprisePanel.scrollIntoView({ behavior: 'smooth', block: 'center' });
    });
};

const initAdminDashboard = () => {
    const admin = document.querySelector('[data-admin-dashboard]');

    if (!admin) {
        return;
    }

    const cards = [...admin.querySelectorAll('.admin-kanban-card')];
    const columns = [...admin.querySelectorAll('.admin-kanban__column')];
    const approvalButtons = [...admin.querySelectorAll('[data-admin-approve]')];

    cards.forEach((card) => {
        card.addEventListener('dragstart', () => {
            card.classList.add('is-dragging');
        });

        card.addEventListener('dragend', () => {
            card.classList.remove('is-dragging');
            columns.forEach((column) => {
                const count = column.querySelectorAll('.admin-kanban-card').length;
                const badge = column.querySelector('h3 span');

                if (badge) {
                    badge.textContent = String(count);
                }
            });
        });
    });

    columns.forEach((column) => {
        column.addEventListener('dragover', (event) => {
            event.preventDefault();
            const draggingCard = admin.querySelector('.admin-kanban-card.is-dragging');

            if (draggingCard) {
                column.appendChild(draggingCard);
            }
        });
    });

    approvalButtons.forEach((button) => {
        button.addEventListener('click', () => {
            const row = button.closest('article');
            const status = row?.querySelector('small');

            button.textContent = 'Approved';
            button.disabled = true;

            if (status) {
                status.textContent = 'Approved for itinerary mapping';
            }
        });
    });
};

document.addEventListener('DOMContentLoaded', () => {
    document.documentElement.classList.add('sg-motion-ready');
    initMountainRange();
    initDestinationCinema();
    initLandingPage();
    initBookingFlow();
    initAccountFlow();
    initDashboard();
    initAdminDashboard();
});
